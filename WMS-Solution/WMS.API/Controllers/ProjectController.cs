using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using WMS.Application.Common;
using WMS.Application.DTOs.Employee;
using WMS.Application.DTOs.Project;
using WMS.Application.Interfaces.Services;
using WMS.Domain.Interfaces;

namespace WMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectController : ControllerBase
{
    private readonly IProjectService
        _projectService;

    private readonly IEmployeeService
        _employeeService;

    private readonly IUnitOfWork
        _unitOfWork;

    public ProjectController(
        IProjectService projectService,
        IEmployeeService employeeService,
        IUnitOfWork unitOfWork)
    {
        _projectService = projectService;

        _employeeService = employeeService;

        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        IEnumerable<ProjectResponseDto> result;

        if (User.IsInRole("Admin"))
        {
            result = await _projectService.GetAllAsync();
        }
        else
        {
            var currentEmployee = await ResolveCurrentEmployeeAsync();

            if (currentEmployee == null)
            {
                return Forbid();
            }

            var allProjects = await _projectService.GetAllAsync();
            var allocations = await _unitOfWork.Allocations.GetAllAsync();

            if (User.IsInRole("Manager"))
            {
                var teamEmployeeIds = (await _employeeService.GetAllEmployeesAsync())
                    .Where(employee => employee.DepartmentId == currentEmployee.DepartmentId)
                    .Select(employee => employee.EmployeeId)
                    .ToHashSet();

                var projectIds = allocations
                    .Where(allocation => teamEmployeeIds.Contains(allocation.EmpId))
                    .Select(allocation => allocation.ProjectId)
                    .ToHashSet();

                result = allProjects.Where(project => projectIds.Contains(project.ProjectId));
            }
            else if (User.IsInRole("Employee"))
            {
                var projectIds = allocations
                    .Where(allocation => allocation.EmpId == currentEmployee.EmployeeId)
                    .Select(allocation => allocation.ProjectId)
                    .ToHashSet();

                result = allProjects.Where(project => projectIds.Contains(project.ProjectId));
            }
            else
            {
                return Forbid();
            }
        }

        return Ok(
            ApiResponse<IEnumerable<
                ProjectResponseDto>>
            .SuccessResponse(result));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(
        CreateProjectDto dto)
    {
        var result =
            await _projectService
                .CreateAsync(dto);

        return Ok(
            ApiResponse<ProjectResponseDto>
            .SuccessResponse(result));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] CreateProjectDto dto)
    {
        var result =
            await _projectService.UpdateAsync(id, dto);

        return Ok(
            ApiResponse<ProjectResponseDto>
            .SuccessResponse(result));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _projectService.DeleteAsync(id);

        return Ok(
            ApiResponse<string>
                .SuccessResponse(
                    string.Empty,
                    "Project deleted successfully."));
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
