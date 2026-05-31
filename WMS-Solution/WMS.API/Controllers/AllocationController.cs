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
        if (dto.EmpId <= 0 || dto.ProjectId <= 0)
        {
            return BadRequest(ApiResponse<AllocationResponseDto>.FailureResponse(
                new List<string> { "Employee and project are required." },
                "Invalid allocation request."));
        }

        var employeeExists = await _unitOfWork.Employees.ExistsAsync(employee => employee.EmployeeId == dto.EmpId);
        var projectExists = await _unitOfWork.Projects.ExistsAsync(project => project.ProjectId == dto.ProjectId);

        if (!employeeExists || !projectExists)
        {
            return NotFound(ApiResponse<AllocationResponseDto>.FailureResponse(
                new List<string> { "Employee or project not found." },
                "Allocation failed."));
        }

        var existingAllocation = (await _unitOfWork.Allocations.GetAllAsync())
            .FirstOrDefault(allocation => allocation.EmpId == dto.EmpId && allocation.ProjectId == dto.ProjectId && allocation.Status);

        if (existingAllocation != null)
        {
            return BadRequest(ApiResponse<AllocationResponseDto>.FailureResponse(
                new List<string> { "This employee is already assigned to this project." },
                "Allocation failed."));
        }

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
        var employees = await _unitOfWork.Employees.GetAllEmployeesWithDetailsAsync();
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
                    RoleName = employee?.Role?.RoleName ?? string.Empty,
                    DepartmentName = employee?.Department?.DepartmentName ?? string.Empty,
                    ProjectId = allocation.ProjectId,
                    ProjectName = project?.ProjectName ?? string.Empty,
                    AssignedOn = allocation.AssignedOn
                };
            })
            .ToList();

        return Ok(ApiResponse<IEnumerable<ProjectAllocationResponseDto>>.SuccessResponse(result));
    }

    [HttpGet("my-project-allocations")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> GetMyProjectAllocations()
    {
        var currentEmployee = await ResolveCurrentEmployeeAsync();

        if (currentEmployee == null)
        {
            return Forbid();
        }

        var allAllocations = await _unitOfWork.Allocations.GetAllAsync();
        var employees = await _unitOfWork.Employees.GetAllEmployeesWithDetailsAsync();
        var projects = await _unitOfWork.Projects.GetAllAsync();

        var myProjectIds = allAllocations
            .Where(a => a.EmpId == currentEmployee.EmployeeId && a.Status)
            .Select(a => a.ProjectId)
            .ToHashSet();

        var result = allAllocations
            .Where(allocation =>
                myProjectIds.Contains(allocation.ProjectId) &&
                allocation.Status &&
                allocation.EmpId != currentEmployee.EmployeeId)
            .Select(allocation =>
            {
                var employee = employees.FirstOrDefault(item => item.EmployeeId == allocation.EmpId);
                var project = projects.FirstOrDefault(item => item.ProjectId == allocation.ProjectId);

                return new ProjectAllocationResponseDto
                {
                    AllocationId = allocation.AllocationId,
                    EmpId = allocation.EmpId,
                    EmployeeName = employee == null ? string.Empty : $"{employee.FirstName} {employee.LastName}",
                    RoleName = employee?.Role?.RoleName ?? string.Empty,
                    DepartmentName = employee?.Department?.DepartmentName ?? string.Empty,
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
