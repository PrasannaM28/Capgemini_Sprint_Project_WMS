using AutoMapper;
using BCrypt.Net;
using Microsoft.Extensions.Logging;
using WMS.Application.Common;
using WMS.Application.DTOs.Employee;
using WMS.Application.Exceptions;
using WMS.Application.Interfaces.Services;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services;

public class EmployeeService : IEmployeeService
{
    private const string DefaultEmployeePassword = "Employee@123";

    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    private readonly ILogger<EmployeeService>
        _logger;

    public EmployeeService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<EmployeeService> logger)
    {
        _unitOfWork = unitOfWork;

        _mapper = mapper;

        _logger = logger;
    }

    public async Task<EmployeeResponseDto>
        CreateEmployeeAsync(
            CreateEmployeeDto dto)
    {
        var emailExists =
            await _unitOfWork
                .Employees
                .ExistsAsync(e =>
                    e.Email == dto.Email);

        if (emailExists)
        {
            _logger.LogWarning(
                "Duplicate email attempted: {Email}",
                dto.Email);

            throw new BadRequestException(
                "Employee email already exists.");
        }

        var loginExists =
            await _unitOfWork
                .UserLogins
                .ExistsAsync(u =>
                    u.Username == dto.Email);

        if (loginExists)
        {
            _logger.LogWarning(
                "Duplicate username attempted for employee login: {Username}",
                dto.Email);

            throw new BadRequestException(
                "Employee login already exists.");
        }

        var employee =
            _mapper.Map<Employee>(dto);

        var userLogin = new UserLogin
        {
            Username = dto.Email,

            PasswordHash = BCrypt.Net.BCrypt
                .HashPassword(DefaultEmployeePassword),

            RoleId = dto.RoleId
        };

        await _unitOfWork
            .Employees
            .AddAsync(employee);

        await _unitOfWork
            .UserLogins
            .AddAsync(userLogin);

        await _unitOfWork
            .SaveChangesAsync();

        _logger.LogInformation(
            "Employee created successfully with Id: {EmployeeId}",
            employee.EmployeeId);

        var createdEmployee =
            await _unitOfWork
                .Employees
                .GetEmployeeWithDepartmentAsync(
                    employee.EmployeeId);

        return _mapper.Map<
            EmployeeResponseDto>(
                createdEmployee);
    }

    public async Task<IEnumerable<
        EmployeeResponseDto>>
        GetAllEmployeesAsync()
    {
        var employees =
            await _unitOfWork
                .Employees
                .GetAllEmployeesWithDetailsAsync();

        return _mapper.Map<
            IEnumerable<EmployeeResponseDto>>
            (employees);
    }

    public async Task<EmployeeResponseDto>
        GetEmployeeByIdAsync(
            int employeeId)
    {
        var employee =
            await _unitOfWork
                .Employees
                .GetEmployeeWithDepartmentAsync(
                    employeeId);

        if (employee == null)
        {
            _logger.LogWarning(
                "Employee not found with Id: {EmployeeId}",
                employeeId);

            throw new NotFoundException(
                "Employee not found.");
        }

        return _mapper.Map<
            EmployeeResponseDto>(
                employee);
    }

    public async Task<EmployeeResponseDto>
        UpdateEmployeeAsync(
            UpdateEmployeeDto dto)
    {
        var employee =
            await _unitOfWork
                .Employees
                .GetByIdAsync(
                    dto.EmployeeId);

        if (employee == null)
        {
            _logger.LogWarning(
                "Employee not found for update with Id: {EmployeeId}",
                dto.EmployeeId);

            throw new NotFoundException(
                "Employee not found.");
        }

        employee.FirstName =
            dto.FirstName;

        employee.LastName =
            dto.LastName;

        employee.PhoneNumber =
            dto.PhoneNumber;

        employee.Gender =
            dto.Gender;

        employee.DepartmentId =
            dto.DepartmentId;

        employee.RoleId =
            dto.RoleId;

        employee.UpdatedOn =
            DateTime.UtcNow;

        _unitOfWork
            .Employees
            .Update(employee);

        await _unitOfWork
            .SaveChangesAsync();

        _logger.LogInformation(
            "Employee updated successfully with Id: {EmployeeId}",
            employee.EmployeeId);

        var updatedEmployee =
            await _unitOfWork
                .Employees
                .GetEmployeeWithDepartmentAsync(
                    employee.EmployeeId);

        return _mapper.Map<
            EmployeeResponseDto>(
                updatedEmployee);
    }

    public async Task DeleteEmployeeAsync(
        int employeeId)
    {
        var employee =
            await _unitOfWork
                .Employees
                .GetByIdAsync(employeeId);

        if (employee == null)
        {
            _logger.LogWarning(
                "Employee not found for delete with Id: {EmployeeId}",
                employeeId);

            throw new NotFoundException(
                "Employee not found.");
        }

        _unitOfWork
            .Employees
            .Remove(employee);

        await _unitOfWork
            .SaveChangesAsync();

        _logger.LogInformation(
            "Employee deleted successfully with Id: {EmployeeId}",
            employee.EmployeeId);
    }

    public async Task<IEnumerable<
        EmployeeResponseDto>>
        SearchEmployeesAsync(
            string searchTerm)
    {
        var employees =
            await _unitOfWork
                .Employees
                .SearchEmployeesAsync(
                    searchTerm);

        return _mapper.Map<
            IEnumerable<EmployeeResponseDto>>
            (employees);
    }

    public async Task<
        PagedResult<EmployeeResponseDto>>
        GetPagedEmployeesAsync(
            PaginationQuery query)
    {
        var employees =
            await _unitOfWork
                .Employees
                .GetAllEmployeesWithDetailsAsync();

        var total =
            employees.Count();

        var paged =
            employees
                .Skip(
                    (query.PageNumber - 1)
                    * query.PageSize)
                .Take(query.PageSize);

        return new PagedResult<
            EmployeeResponseDto>
        {
            Items =
                _mapper.Map<
                    IEnumerable<
                        EmployeeResponseDto>>
                (paged),

            TotalRecords = total,

            PageNumber = query.PageNumber,

            PageSize = query.PageSize
        };
    }
}
