using AutoMapper;
using FluentAssertions;
using Moq;
using WMS.Application.DTOs.Allocation;
using WMS.Application.Mappings;
using WMS.Application.Services;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Tests.Services;

public class AllocationServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    private readonly Mock<IAllocationRepository> _allocationRepositoryMock;

    private readonly IMapper _mapper;

    private readonly AllocationService _allocationService;

    public AllocationServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _allocationRepositoryMock = new Mock<IAllocationRepository>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

        _mapper = mapperConfig.CreateMapper();

        _unitOfWorkMock
            .Setup(x => x.Allocations)
            .Returns(_allocationRepositoryMock.Object);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _allocationService = new AllocationService(
            _unitOfWorkMock.Object,
            _mapper);
    }

    [Fact]
    public async Task AllocateAsync_ShouldCreateAllocation_WhenNoExistingPairExists()
    {
        var dto = new CreateAllocationDto
        {
            EmpId = 6,
            ProjectId = 1,
            CreatedBy = "manager@test.com"
        };

        _allocationRepositoryMock
            .Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<EmployeeProjectAllocation, bool>>>()))
            .ReturnsAsync(Array.Empty<EmployeeProjectAllocation>());

        EmployeeProjectAllocation? addedAllocation = null;

        _allocationRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<EmployeeProjectAllocation>()))
            .Callback<EmployeeProjectAllocation>(allocation => addedAllocation = allocation)
            .Returns(Task.CompletedTask);

        var result = await _allocationService.AllocateAsync(dto);

        result.Should().NotBeNull();
        result.EmpId.Should().Be(dto.EmpId);
        result.ProjectId.Should().Be(dto.ProjectId);

        addedAllocation.Should().NotBeNull();
        addedAllocation!.Status.Should().BeTrue();

        _allocationRepositoryMock.Verify(x => x.Update(It.IsAny<EmployeeProjectAllocation>()), Times.Never);
        _allocationRepositoryMock.Verify(x => x.AddAsync(It.IsAny<EmployeeProjectAllocation>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AllocateAsync_ShouldReactivateExistingAllocation_WhenInactiveRowExists()
    {
        var existingAllocation = new EmployeeProjectAllocation
        {
            AllocationId = 12,
            EmpId = 6,
            ProjectId = 1,
            Status = false,
            CreatedBy = "manager@test.com",
            CreateDate = DateTime.UtcNow.AddDays(-3),
            AssignedOn = DateTime.UtcNow.AddDays(-3)
        };

        var dto = new CreateAllocationDto
        {
            EmpId = 6,
            ProjectId = 1,
            CreatedBy = "manager@test.com"
        };

        _allocationRepositoryMock
            .Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<EmployeeProjectAllocation, bool>>>()))
            .ReturnsAsync(new[] { existingAllocation });

        var result = await _allocationService.AllocateAsync(dto);

        result.Should().NotBeNull();
        result.AllocationId.Should().Be(existingAllocation.AllocationId);
        result.EmpId.Should().Be(dto.EmpId);
        result.ProjectId.Should().Be(dto.ProjectId);

        existingAllocation.Status.Should().BeTrue();
        existingAllocation.UpdatedBy.Should().Be(dto.CreatedBy);
        existingAllocation.UpdatedDate.Should().NotBeNull();

        _allocationRepositoryMock.Verify(x => x.AddAsync(It.IsAny<EmployeeProjectAllocation>()), Times.Never);
        _allocationRepositoryMock.Verify(x => x.Update(existingAllocation), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}