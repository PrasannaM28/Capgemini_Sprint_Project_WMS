using WMS.Application.DTOs.Employee;
using WMS.Application.Common;

namespace WMS.Application.Interfaces.Services;

public interface IEmployeeService
{
    Task<EmployeeResponseDto> CreateEmployeeAsync(
        CreateEmployeeDto dto);

    Task<IEnumerable<EmployeeResponseDto>> GetAllEmployeesAsync();

    Task<EmployeeResponseDto> GetEmployeeByIdAsync(int employeeId);

    Task<EmployeeResponseDto> UpdateEmployeeAsync(
        UpdateEmployeeDto dto);

    Task DeleteEmployeeAsync(int employeeId);

    Task<IEnumerable<EmployeeResponseDto>> SearchEmployeesAsync(
        string searchTerm);

    Task<PagedResult<EmployeeResponseDto>>
    GetPagedEmployeesAsync(
        PaginationQuery query);
}
