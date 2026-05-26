using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WMS.Application.Common;
using WMS.Application.DTOs.Allocation;
using WMS.Application.DTOs.Employee;
using WMS.Application.Interfaces.Services;
using WMS.Domain.Interfaces;

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

    private readonly IUnitOfWork _unitOfWork;

    public AllocationController(
        IAllocationService allocationService,
        IEmployeeService employeeService,
        IUnitOfWork unitOfWork)
    {
        _allocationService = allocationService;

        _employeeService = employeeService;

        _unitOfWork = unitOfWork;
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

    [HttpGet("project/{projectId:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetProjectAllocations(int projectId)
    {
        var allocations = await _unitOfWork.Allocations.GetAllAsync();
        var employees = await _unitOfWork.Employees.GetAllAsync();
        var projects = await _unitOfWork.Projects.GetAllAsync();

        var result = allocations
            .Where(allocation => allocation.ProjectId == projectId && allocation.Status)
            .Select(allocation =>
            {
                var employee = employees.FirstOrDefault(item => item.EmployeeId == allocation.EmpId);
                var project = projects.FirstOrDefault(item => item.ProjectId == allocation.ProjectId);

                return new ProjectAllocationResponseDto
                {
                    AllocationId = allocation.AllocationId,
                    EmpId = allocation.EmpId,
                    EmployeeName = employee == null ? string.Empty : $"{employee.FirstName} {employee.LastName}",
                    ProjectId = allocation.ProjectId,
                    ProjectName = project?.ProjectName ?? string.Empty,
                    AssignedOn = allocation.AssignedOn
                };
            })
            .ToList();

        return Ok(ApiResponse<IEnumerable<ProjectAllocationResponseDto>>.SuccessResponse(result));
    }

    [HttpDelete("project/{projectId:int}/employee/{empId:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Deassign(int projectId, int empId)
    {
        var allocation = (await _unitOfWork.Allocations.GetAllAsync())
            .FirstOrDefault(item => item.ProjectId == projectId && item.EmpId == empId && item.Status);

        if (allocation == null)
        {
            return NotFound(ApiResponse<string>.FailureResponse(new List<string> { "Allocation not found." }));
        }

        allocation.Status = false;
        allocation.UpdatedBy = User.Identity?.Name;
        allocation.UpdatedDate = DateTime.UtcNow;

        _unitOfWork.Allocations.Update(allocation);
        await _unitOfWork.SaveChangesAsync();

        return Ok(ApiResponse<string>.SuccessResponse(string.Empty, "Team member deassigned successfully."));
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
