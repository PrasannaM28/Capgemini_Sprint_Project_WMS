using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WMS.Application.DTOs.Employee;
using WMS.Application.Exceptions;
using WMS.Application.Mappings;
using WMS.Application.Services;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Tests.Services;

public class EmployeeServiceTests
{
    private readonly Mock<IUnitOfWork>
        _unitOfWorkMock;

    private readonly Mock<IEmployeeRepository>
        _employeeRepositoryMock;

    private readonly Mock<
        ILogger<EmployeeService>>
        _loggerMock;

    private readonly IMapper _mapper;

    private readonly EmployeeService
        _employeeService;

    public EmployeeServiceTests()
    {
        _unitOfWorkMock =
            new Mock<IUnitOfWork>();

        _employeeRepositoryMock =
            new Mock<IEmployeeRepository>();

        _loggerMock =
            new Mock<
                ILogger<EmployeeService>>();

        var mapperConfig =
            new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<
                    MappingProfile>();
            });

        _mapper =
            mapperConfig.CreateMapper();

        _unitOfWorkMock
            .Setup(x => x.Employees)
            .Returns(
                _employeeRepositoryMock
                    .Object);

        _employeeService =
            new EmployeeService(
                _unitOfWorkMock.Object,
                _mapper,
                _loggerMock.Object);
    }

    [Fact]
    public async Task
        GetEmployeeByIdAsync_ShouldReturnEmployee()
    {
        // Arrange

        var employee = new Employee
        {
            EmployeeId = 1,

            FirstName = "Prasanna",

            LastName = "Moharil",

            Email = "prasanna@test.com"
        };

        _employeeRepositoryMock
            .Setup(x =>
                x.GetEmployeeWithDepartmentAsync(
                    1))
            .ReturnsAsync(employee);

        // Act

        var result =
            await _employeeService
                .GetEmployeeByIdAsync(1);

        // Assert

        result.Should().NotBeNull();

        result.EmployeeId.Should().Be(1);

        result.Email.Should()
            .Be("prasanna@test.com");
    }

    [Fact]
    public async Task
        GetEmployeeByIdAsync_ShouldThrowNotFoundException()
    {
        // Arrange

        _employeeRepositoryMock
            .Setup(x =>
                x.GetEmployeeWithDepartmentAsync(
                    100))
            .ReturnsAsync(
                (Employee?)null);

        // Act

        Func<Task> act = async () =>
        {
            await _employeeService
                .GetEmployeeByIdAsync(100);
        };

        // Assert

        await act.Should()
            .ThrowAsync<
                NotFoundException>();
    }
}
