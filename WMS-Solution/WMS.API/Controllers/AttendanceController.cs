using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WMS.Application.Common;
using WMS.Application.DTOs.Attendance;
using WMS.Application.DTOs.Employee;
using WMS.Application.Interfaces.Services;

namespace WMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService
        _attendanceService;

    private readonly IEmployeeService
        _employeeService;

    public AttendanceController(
        IAttendanceService attendanceService,
        IEmployeeService employeeService)
    {
        _attendanceService = attendanceService;

        _employeeService = employeeService;
    }

    [HttpPost("checkin")]
    [Authorize(Roles = "Admin,Manager,Employee")]
    public async Task<IActionResult> CheckIn(
        CreateAttendanceDto dto)
    {
        var currentEmployee =
            await ResolveCurrentEmployeeAsync();

        if (currentEmployee == null ||
            dto.EmpId != currentEmployee.EmployeeId)
        {
            return Forbid();
        }

        var result =
            await _attendanceService
                .CheckInAsync(dto);

        return Ok(
            ApiResponse<AttendanceResponseDto>
            .SuccessResponse(result));
    }

    [HttpPut("checkout")]
    [Authorize(Roles = "Admin,Manager,Employee")]
    public async Task<IActionResult> CheckOut(
        CheckoutAttendanceDto dto)
    {
        var currentEmployee =
            await ResolveCurrentEmployeeAsync();

        if (currentEmployee == null)
        {
            return Forbid();
        }

        var attendance =
            await _attendanceService
                .GetAttendanceByIdAsync(dto.AttendanceId);

        if (attendance == null ||
            attendance.EmpId != currentEmployee.EmployeeId)
        {
            return Forbid();
        }

        var result =
            await _attendanceService
                .CheckOutAsync(dto);

        return Ok(
            ApiResponse<AttendanceResponseDto>
            .SuccessResponse(result));
    }

    [HttpGet("monthly")]
    public async Task<IActionResult>
        GetMonthlyAttendance(
            int employeeId,
            int month,
            int year)
    {
        if (User.IsInRole("Employee"))
        {
            var currentEmployee =
                await ResolveCurrentEmployeeAsync();

            if (currentEmployee == null)
            {
                return Forbid();
            }

            employeeId = currentEmployee.EmployeeId;
        }
        else if (User.IsInRole("Manager"))
        {
            var currentEmployee =
                await ResolveCurrentEmployeeAsync();

            if (currentEmployee == null)
            {
                return Forbid();
            }

            var employees = await _employeeService
                .GetAllEmployeesAsync();

            var teamEmployeeIds = employees
                .Where(e =>
                    e.DepartmentId == currentEmployee.DepartmentId)
                .Select(e => e.EmployeeId)
                .ToHashSet();

            if (!teamEmployeeIds.Contains(employeeId))
            {
                return Forbid();
            }
        }

        var result =
            await _attendanceService
                .GetMonthlyAttendanceAsync(
                    employeeId,
                    month,
                    year);

        return Ok(
            ApiResponse<IEnumerable<
                AttendanceResponseDto>>
            .SuccessResponse(result));
    }

    private async Task<EmployeeResponseDto?>
        ResolveCurrentEmployeeAsync()
    {
        var username = User.FindFirstValue(JwtRegisteredClaimNames.UniqueName)
            ?? User.FindFirstValue(ClaimTypes.Name)
            ?? User.Identity?.Name;

        if (string.IsNullOrWhiteSpace(username))
        {
            return null;
        }

        var candidates = await _employeeService
            .SearchEmployeesAsync(username);

        return candidates.FirstOrDefault(e =>
            string.Equals(
                e.Email,
                username,
                StringComparison.OrdinalIgnoreCase));
    }
}
