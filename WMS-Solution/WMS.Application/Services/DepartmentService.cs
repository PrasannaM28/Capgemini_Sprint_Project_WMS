using AutoMapper;
using WMS.Application.Exceptions;
using WMS.Application.DTOs.Department;
using WMS.Application.Interfaces.Services;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services;

public class DepartmentService
    : IDepartmentService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    public DepartmentService(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;

        _mapper = mapper;
    }

    public async Task<IEnumerable<DepartmentResponseDto>>
        GetAllAsync()
    {
        var departments =
            await _unitOfWork
                .Departments
                .GetAllAsync();

        return _mapper.Map<
            IEnumerable<DepartmentResponseDto>>
            (departments);
    }

    public async Task<DepartmentResponseDto>
        CreateAsync(CreateDepartmentDto dto)
    {
        var department =
            _mapper.Map<Department>(dto);

        await _unitOfWork
            .Departments
            .AddAsync(department);

        await _unitOfWork
            .SaveChangesAsync();

        return _mapper.Map<
            DepartmentResponseDto>(department);
    }

    public async Task<DepartmentResponseDto>
        UpdateAsync(int departmentId, CreateDepartmentDto dto)
    {
        var department =
            await _unitOfWork
                .Departments
                .GetByIdAsync(departmentId);

        if (department == null)
        {
            throw new NotFoundException(
                "Department not found.");
        }

        department.DepartmentName =
            dto.DepartmentName;

        department.Description =
            dto.Description;

        _unitOfWork.Departments.Update(department);

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<DepartmentResponseDto>(department);
    }

    public async Task DeleteAsync(int departmentId)
    {
        var department =
            await _unitOfWork
                .Departments
                .GetByIdAsync(departmentId);

        if (department == null)
        {
            throw new NotFoundException(
                "Department not found.");
        }

        _unitOfWork.Departments.Remove(department);

        await _unitOfWork.SaveChangesAsync();
    }
}
