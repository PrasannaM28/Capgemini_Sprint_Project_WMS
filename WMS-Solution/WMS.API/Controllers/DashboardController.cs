using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using WMS.Application.Common;
using WMS.Application.DTOs.Dashboard;
using WMS.Application.DTOs.Employee;
using WMS.Application.Interfaces.Services;

namespace WMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController
    : ControllerBase
{
    private readonly IDashboardService
        _dashboardService;

    private readonly IEmployeeService
        _employeeService;

    public DashboardController(
        IDashboardService dashboardService,
        IEmployeeService employeeService)
    {
        _dashboardService =
            dashboardService;

        _employeeService = employeeService;
    }

    [HttpGet]
    public async Task<IActionResult>
        GetDashboard()
    {
        var role = User.IsInRole("Admin")
            ? "Admin"
            : User.IsInRole("Manager")
                ? "Manager"
                : User.IsInRole("Employee")
                    ? "Employee"
                    : string.Empty;

        var currentEmployee = await ResolveCurrentEmployeeAsync();

        var result =
            await _dashboardService
                .GetDashboardAsync(
                    role,
                    currentEmployee?.EmployeeId,
                    currentEmployee?.DepartmentId);

        return Ok(
            ApiResponse<
                DashboardResponseDto>
            .SuccessResponse(result));
    }

    private async Task<EmployeeResponseDto?> ResolveCurrentEmployeeAsync()
    {
        var username = User.FindFirstValue(JwtRegisteredClaimNames.UniqueName)
            ?? User.FindFirstValue(ClaimTypes.Name)
            ?? User.Identity?.Name;

        if (string.IsNullOrWhiteSpace(username))
        {
            return null;
        }

        var candidates = await _employeeService.SearchEmployeesAsync(username);

        return candidates.FirstOrDefault(employee =>
            string.Equals(employee.Email, username, StringComparison.OrdinalIgnoreCase));
    }
}
