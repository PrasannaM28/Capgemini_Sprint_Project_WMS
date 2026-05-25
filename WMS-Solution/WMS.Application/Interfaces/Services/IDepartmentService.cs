using WMS.Application.DTOs.Department;

namespace WMS.Application.Interfaces.Services;

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentResponseDto>>
        GetAllAsync();

    Task<DepartmentResponseDto>
        CreateAsync(CreateDepartmentDto dto);

    Task<DepartmentResponseDto>
        UpdateAsync(int departmentId, CreateDepartmentDto dto);

    Task DeleteAsync(int departmentId);
}
