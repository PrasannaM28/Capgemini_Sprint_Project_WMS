using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using WMS.Application.DTOs.Leave;
using WMS.Application.Mappings;
using WMS.Application.Services;
using WMS.Domain.Interfaces;

namespace WMS.Tests.Services;

public class LeaveServiceTests
{
    private readonly Mock<IUnitOfWork>
        _unitOfWorkMock;

    private readonly Mock<ILeaveRepository>
        _leaveRepositoryMock;

    private readonly Mock<
        ILogger<LeaveService>>
        _loggerMock;

    private readonly IMapper _mapper;

    private readonly LeaveService
        _leaveService;

    public LeaveServiceTests()
    {
        _unitOfWorkMock =
            new Mock<IUnitOfWork>();

        _leaveRepositoryMock =
            new Mock<ILeaveRepository>();

        _loggerMock =
            new Mock<
                ILogger<LeaveService>>();

        var mapperConfig =
            new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<
                    MappingProfile>();
            });

        _mapper =
            mapperConfig.CreateMapper();

        _unitOfWorkMock
            .Setup(x => x.Leaves)
            .Returns(
                _leaveRepositoryMock
                    .Object);

        _leaveService =
            new LeaveService(
                _unitOfWorkMock.Object,
                _mapper,
                _loggerMock.Object);
    }

    [Fact]
    public async Task
        ApplyLeaveAsync_ShouldCreateLeave()
    {
        // Arrange

        var dto =
            new CreateLeaveDto
            {
                EmpId = 1,

                LeaveType = "Sick Leave",

                FromDate = DateTime.Now,

                ToDate = DateTime.Now.AddDays(2)
            };

        // Act

        var result =
            await _leaveService
                .ApplyLeaveAsync(dto);

        // Assert

        Assert.NotNull(result);

        Assert.Equal(
            "Sick Leave",
            result.LeaveType);
    }
}
