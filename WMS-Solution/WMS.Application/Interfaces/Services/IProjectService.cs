using WMS.Application.DTOs.Project;

namespace WMS.Application.Interfaces.Services;

public interface IProjectService
{
    Task<ProjectResponseDto>
        CreateAsync(CreateProjectDto dto);

    Task<IEnumerable<ProjectResponseDto>>
        GetAllAsync();

    Task<ProjectResponseDto>
        UpdateAsync(int projectId, CreateProjectDto dto);

    Task DeleteAsync(int projectId);
}
