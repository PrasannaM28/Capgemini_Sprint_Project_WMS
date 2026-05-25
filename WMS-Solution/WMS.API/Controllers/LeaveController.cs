using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using WMS.Application.Common;
using WMS.Application.DTOs.Employee;
using WMS.Application.DTOs.Leave;
using WMS.Application.Interfaces.Services;
using WMS.Domain.Interfaces;

namespace WMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LeaveController : ControllerBase
{
    private readonly ILeaveService
        _leaveService;

    private readonly IEmployeeService
        _employeeService;

    private readonly IUnitOfWork
        _unitOfWork;

    public LeaveController(
        ILeaveService leaveService,
        IEmployeeService employeeService,
        IUnitOfWork unitOfWork)
    {
        _leaveService = leaveService;

        _employeeService = employeeService;

        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        IEnumerable<LeaveResponseDto> result;

        if (User.IsInRole("Admin"))
        {
            result = await _leaveService.GetAllAsync();
        }
        else
        {
            var currentEmployee = await ResolveCurrentEmployeeAsync();

            if (currentEmployee == null)
            {
                return Forbid();
            }

            var allLeaves = await _leaveService.GetAllAsync();

            if (User.IsInRole("Manager"))
            {
                var teamEmployeeIds =
                    await GetSharedProjectEmployeeIdsAsync(
                        currentEmployee.EmployeeId);

                result = allLeaves.Where(leave =>
                    teamEmployeeIds.Contains(leave.EmpId));
            }
            else if (User.IsInRole("Employee"))
            {
                result = allLeaves.Where(leave => leave.EmpId == currentEmployee.EmployeeId);
            }
            else
            {
                return Forbid();
            }
        }

        return Ok(
            ApiResponse<IEnumerable<LeaveResponseDto>>
            .SuccessResponse(result));
    }

    [HttpPost]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> ApplyLeave(
        CreateLeaveDto dto)
    {
        var currentEmployee = await ResolveCurrentEmployeeAsync();

        if (currentEmployee == null)
        {
            return Forbid();
        }

        dto.EmpId = currentEmployee.EmployeeId;

        var result =
            await _leaveService
                .ApplyLeaveAsync(dto);

        return Ok(
            ApiResponse<LeaveResponseDto>
            .SuccessResponse(result));
    }

    [HttpPut("approve")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> ApproveLeave(
        LeaveApprovalDto dto)
    {
        if (User.IsInRole("Manager"))
        {
            var currentEmployee = await ResolveCurrentEmployeeAsync();

            if (currentEmployee == null)
            {
                return Forbid();
            }

            var leave = await _leaveService.GetByIdAsync(dto.LeaveId);

            if (leave == null)
            {
                return NotFound();
            }

            var teamEmployeeIds = await GetSharedProjectEmployeeIdsAsync(
                currentEmployee.EmployeeId);

            if (!teamEmployeeIds.Contains(leave.EmpId))
            {
                return Forbid();
            }
        }

        var result =
            await _leaveService
                .ApproveLeaveAsync(dto);

        return Ok(
            ApiResponse<LeaveResponseDto>
            .SuccessResponse(result));
    }

    [HttpPut("cancel/{leaveId}")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> CancelLeave(
        int leaveId)
    {
        var currentEmployee = await ResolveCurrentEmployeeAsync();

        if (currentEmployee == null)
        {
            return Forbid();
        }

        var leave = await _leaveService.GetByIdAsync(leaveId);

        if (leave == null || leave.EmpId != currentEmployee.EmployeeId)
        {
            return Forbid();
        }

        await _leaveService
            .CancelLeaveAsync(leaveId);

        return Ok(
            ApiResponse<string>
                .SuccessResponse(
                    string.Empty,
                    "Leave cancelled."));
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

    private async Task<HashSet<int>> GetSharedProjectEmployeeIdsAsync(
        int employeeId)
    {
        var allocations = await _unitOfWork.Allocations.GetAllAsync();

        var currentEmployeeProjectIds = allocations
            .Where(allocation =>
                allocation.EmpId == employeeId && allocation.Status)
            .Select(allocation => allocation.ProjectId)
            .ToHashSet();

        if (currentEmployeeProjectIds.Count == 0)
        {
            return new HashSet<int>();
        }

        return allocations
            .Where(allocation =>
                allocation.Status &&
                allocation.EmpId != employeeId &&
                currentEmployeeProjectIds.Contains(allocation.ProjectId))
            .Select(allocation => allocation.EmpId)
            .ToHashSet();
    }
}
