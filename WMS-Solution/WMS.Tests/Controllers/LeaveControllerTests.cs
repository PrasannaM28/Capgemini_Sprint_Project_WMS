using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using WMS.API.Controllers;
using WMS.Application.Common;
using WMS.Application.DTOs.Employee;
using WMS.Application.DTOs.Leave;
using WMS.Application.Interfaces.Services;
using WMS.Domain.Enums;
using WMS.Domain.Interfaces;

namespace WMS.Tests.Controllers;

public class LeaveControllerTests
{
    [Fact]
    public async Task ApproveLeave_ShouldUseAuthenticatedEmployeeId_ForApprovedByAndName()
    {
        var leaveServiceMock = new Mock<ILeaveService>();
        var employeeServiceMock = new Mock<IEmployeeService>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var approver = new EmployeeResponseDto
        {
            EmployeeId = 42,
            FirstName = "Admin",
            LastName = "User",
            FullName = "Admin User",
            Email = "admin@company.com"
        };

        var capturedDto = new LeaveApprovalDto();

        employeeServiceMock
            .Setup(service => service.SearchEmployeesAsync("admin@company.com"))
            .ReturnsAsync(new[] { approver });

        employeeServiceMock
            .Setup(service => service.GetAllEmployeesAsync())
            .ReturnsAsync(new[] { approver });

        leaveServiceMock
            .Setup(service => service.ApproveLeaveAsync(It.IsAny<LeaveApprovalDto>()))
            .Callback<LeaveApprovalDto>(dto => capturedDto = dto)
            .ReturnsAsync(() => new LeaveResponseDto
            {
                LeaveId = 7,
                EmpId = 11,
                LeaveType = "Sick Leave",
                Status = LeaveStatus.Approved.ToString(),
                AppliedOn = DateTime.UtcNow,
                ApprovedBy = capturedDto.ApprovedBy,
                ApprovedOn = DateTime.UtcNow
            });

        var controller = new LeaveController(
            leaveServiceMock.Object,
            employeeServiceMock.Object,
            unitOfWorkMock.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "admin@company.com"),
                    new Claim(ClaimTypes.Role, "Admin")
                }, "TestAuth"))
            }
        };

        var result = await controller.ApproveLeave(new LeaveApprovalDto
        {
            LeaveId = 7,
            Status = LeaveStatus.Approved,
            ApprovedBy = 999
        });

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<LeaveResponseDto>>(okResult.Value);

        Assert.True(response.Success);
        Assert.Equal(42, capturedDto.ApprovedBy);
        Assert.Equal("Admin User", response.Data?.ApprovedByName);
        Assert.Equal(42, response.Data?.ApprovedBy);
    }
}