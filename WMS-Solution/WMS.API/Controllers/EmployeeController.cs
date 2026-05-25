using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common;
using WMS.Application.DTOs.Employee;
using WMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeeController(
        IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllEmployees()
    {
        IEnumerable<EmployeeResponseDto> employees;

        if (User.IsInRole("Admin"))
        {
            employees = await _employeeService
                .GetAllEmployeesAsync();
        }
        else if (User.IsInRole("Manager"))
        {
            var currentEmployee =
                await ResolveCurrentEmployeeAsync();

            if (currentEmployee == null)
            {
                return Forbid();
            }

            employees = new List<EmployeeResponseDto>
            {
                currentEmployee
            };
        }
        else if (User.IsInRole("Employee"))
        {
            var currentEmployee =
                await ResolveCurrentEmployeeAsync();

            if (currentEmployee == null)
            {
                return Forbid();
            }

            employees = new List<EmployeeResponseDto>
            {
                currentEmployee
            };
        }
        else
        {
            return Forbid();
        }

        var response = ApiResponse<
            IEnumerable<EmployeeResponseDto>>
            .SuccessResponse(
                employees,
                "Employees fetched successfully.");

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetEmployeeById(
        int id)
    {
        var currentEmployee =
            await ResolveCurrentEmployeeAsync();

        if (User.IsInRole("Manager"))
        {
            if (currentEmployee == null)
            {
                return Forbid();
            }

            var allEmployees = await _employeeService
                .GetAllEmployeesAsync();

            var teamEmployeeIds = allEmployees
                .Where(e =>
                    e.DepartmentId == currentEmployee.DepartmentId)
                .Select(e => e.EmployeeId)
                .ToHashSet();

            if (!teamEmployeeIds.Contains(id))
            {
                return Forbid();
            }
        }

        if (User.IsInRole("Employee"))
        {
            if (currentEmployee == null ||
                currentEmployee.EmployeeId != id)
            {
                return Forbid();
            }
        }

        var employee = await _employeeService
            .GetEmployeeByIdAsync(id);

        var response = ApiResponse<EmployeeResponseDto>
            .SuccessResponse(
                employee,
                "Employee fetched successfully.");

        return Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateEmployee(
        [FromBody] CreateEmployeeDto dto)
    {
        var employee = await _employeeService
            .CreateEmployeeAsync(dto);

        var response = ApiResponse<EmployeeResponseDto>
            .SuccessResponse(
                employee,
                "Employee created successfully.");

        return CreatedAtAction(
            nameof(GetEmployeeById),
            new { id = employee.EmployeeId },
            response);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> UpdateEmployee(
        int id,
        [FromBody] UpdateEmployeeDto dto)
    {
        if (id != dto.EmployeeId)
        {
            return BadRequest(
                ApiResponse<object>.FailureResponse(
                    new List<string>
                    {
                        "Route id and DTO id do not match."
                    },
                    "Invalid request."));
        }

        if (User.IsInRole("Employee"))
        {
            var currentEmployee =
                await ResolveCurrentEmployeeAsync();

            if (currentEmployee == null ||
                currentEmployee.EmployeeId != id)
            {
                return Forbid();
            }

            dto.RoleId = currentEmployee.RoleId;
            dto.DepartmentId = currentEmployee.DepartmentId;
        }

        var employee = await _employeeService
            .UpdateEmployeeAsync(dto);

        var response = ApiResponse<EmployeeResponseDto>
            .SuccessResponse(
                employee,
                "Employee updated successfully.");

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteEmployee(
        int id)
    {
        await _employeeService
            .DeleteEmployeeAsync(id);

        var response = ApiResponse<string>
            .SuccessResponse(
                "Deleted",
                "Employee deleted successfully.");

        return Ok(response);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchEmployees(
        [FromQuery] string searchTerm)
    {
        IEnumerable<EmployeeResponseDto> employees;

        if (User.IsInRole("Admin"))
        {
            employees = await _employeeService
                .SearchEmployeesAsync(searchTerm);
        }
        else if (User.IsInRole("Manager"))
        {
            var currentEmployee =
                await ResolveCurrentEmployeeAsync();

            if (currentEmployee == null)
            {
                return Forbid();
            }

            var term = searchTerm.Trim();

            if (string.IsNullOrEmpty(term) ||
                currentEmployee.FullName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                currentEmployee.Email.Contains(term, StringComparison.OrdinalIgnoreCase))
            {
                employees = new List<EmployeeResponseDto>
                {
                    currentEmployee
                };
            }
            else
            {
                employees = Enumerable.Empty<EmployeeResponseDto>();
            }
        }
        else if (User.IsInRole("Employee"))
        {
            var currentEmployee =
                await ResolveCurrentEmployeeAsync();

            if (currentEmployee == null)
            {
                return Forbid();
            }

            var term = searchTerm.Trim();

            if (string.IsNullOrEmpty(term) ||
                currentEmployee.FullName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                currentEmployee.Email.Contains(term, StringComparison.OrdinalIgnoreCase))
            {
                employees = new List<EmployeeResponseDto>
                {
                    currentEmployee
                };
            }
            else
            {
                employees = Enumerable.Empty<EmployeeResponseDto>();
            }
        }
        else
        {
            return Forbid();
        }

        var response = ApiResponse<
            IEnumerable<EmployeeResponseDto>>
            .SuccessResponse(
                employees,
                "Employees fetched successfully.");

        return Ok(response);
    }

    [HttpGet("paged")]
    public async Task<IActionResult>
    GetPaged(
        [FromQuery]
        PaginationQuery query)
    {
        if (User.IsInRole("Manager"))
        {
            var currentEmployee =
                await ResolveCurrentEmployeeAsync();

            if (currentEmployee == null)
            {
                return Forbid();
            }

            var pagedTeam = new List<EmployeeResponseDto>
            {
                currentEmployee
            };

            var managerResult = new PagedResult<EmployeeResponseDto>
            {
                Items = pagedTeam,
                TotalRecords = 1,
                PageNumber = 1,
                PageSize = 1
            };

            return Ok(
                ApiResponse<
                    PagedResult<
                        EmployeeResponseDto>>
                .SuccessResponse(managerResult));
        }

        if (User.IsInRole("Employee"))
        {
            var currentEmployee =
                await ResolveCurrentEmployeeAsync();

            if (currentEmployee == null)
            {
                return Forbid();
            }

            var employeeResult = new PagedResult<EmployeeResponseDto>
            {
                Items = new List<EmployeeResponseDto>
                {
                    currentEmployee
                },
                TotalRecords = 1,
                PageNumber = 1,
                PageSize = 1
            };

            return Ok(
                ApiResponse<
                    PagedResult<
                        EmployeeResponseDto>>
                .SuccessResponse(employeeResult));
        }

        var result =
            await _employeeService
                .GetPagedEmployeesAsync(
                    query);

        return Ok(
            ApiResponse<
                PagedResult<
                    EmployeeResponseDto>>
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
