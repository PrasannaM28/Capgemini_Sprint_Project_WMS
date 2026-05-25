using AutoMapper;
using WMS.Application.Exceptions;
using WMS.Application.DTOs.Project;
using WMS.Application.Interfaces.Services;
using WMS.Domain.Entities;
using WMS.Domain.Enums;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services;

public class ProjectService
    : IProjectService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    public ProjectService(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;

        _mapper = mapper;
    }

    public async Task<ProjectResponseDto>
        CreateAsync(CreateProjectDto dto)
    {
        var project =
            _mapper.Map<Project>(dto);

        project.Status = dto.Status ?? ProjectStatus.Active;

        await _unitOfWork
            .Projects
            .AddAsync(project);

        await _unitOfWork
            .SaveChangesAsync();

        return _mapper.Map<
            ProjectResponseDto>(project);
    }

    public async Task<IEnumerable<ProjectResponseDto>>
        GetAllAsync()
    {
        var projects =
            await _unitOfWork
                .Projects
                .GetAllAsync();

        var clientMap = (await _unitOfWork
                .Clients
                .GetAllAsync())
            .ToDictionary(client => client.ClientId, client => client.ClientName);

        var projectDtos = _mapper.Map<List<ProjectResponseDto>>(projects);

        foreach (var projectDto in projectDtos)
        {
            if (projectDto.ClientId.HasValue &&
                clientMap.TryGetValue(projectDto.ClientId.Value, out var clientName))
            {
                projectDto.ClientName = clientName;
            }
        }

        return projectDtos;
    }

    public async Task<ProjectResponseDto>
        UpdateAsync(int projectId, CreateProjectDto dto)
    {
        var project =
            await _unitOfWork
                .Projects
                .GetByIdAsync(projectId);

        if (project == null)
        {
            throw new NotFoundException(
                "Project not found.");
        }

        project.ProjectName = dto.ProjectName;
        project.ClientId = dto.ClientId;
        project.StartDate = dto.StartDate;
        project.EndDate = dto.EndDate;
        project.Status = dto.Status ?? ProjectStatus.Active;

        _unitOfWork.Projects.Update(project);

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ProjectResponseDto>(project);
    }

    public async Task DeleteAsync(int projectId)
    {
        var project =
            await _unitOfWork
                .Projects
                .GetByIdAsync(projectId);

        if (project == null)
        {
            throw new NotFoundException(
                "Project not found.");
        }

        _unitOfWork.Projects.Remove(project);

        await _unitOfWork.SaveChangesAsync();
    }
}
