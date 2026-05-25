using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WMS.Application.Common;
using WMS.Application.DTOs.Allocation;
using WMS.Application.DTOs.Employee;
using WMS.Application.Interfaces.Services;

namespace WMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AllocationController
    : ControllerBase
{
    private readonly IAllocationService
        _allocationService;

    private readonly IEmployeeService
        _employeeService;

    public AllocationController(
        IAllocationService allocationService,
        IEmployeeService employeeService)
    {
        _allocationService = allocationService;

        _employeeService = employeeService;
    }

    [HttpGet("assignable-employees")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAssignableEmployees()
    {
        var employees = await _employeeService.GetAllEmployeesAsync();

        var assignableEmployees = employees
            .Where(employee =>
                !string.Equals(employee.RoleName, "Admin", StringComparison.OrdinalIgnoreCase));

        return Ok(
            ApiResponse<IEnumerable<EmployeeResponseDto>>
                .SuccessResponse(assignableEmployees, "Assignable employees fetched successfully."));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Allocate(
        CreateAllocationDto dto)
    {
        var result =
            await _allocationService
                .AllocateAsync(dto);

        return Ok(
            ApiResponse<AllocationResponseDto>
            .SuccessResponse(result, "Team member assigned successfully."));
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
