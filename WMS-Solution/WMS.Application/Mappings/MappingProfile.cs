using AutoMapper;
using WMS.Application.DTOs.Employee;
using WMS.Domain.Entities;
using WMS.Application.DTOs.Department;
using WMS.Application.DTOs.Attendance;
using WMS.Application.DTOs.Leave;
using WMS.Application.DTOs.Project;
using WMS.Application.DTOs.Client;
using WMS.Application.DTOs.Allocation;
using WMS.Application.DTOs.Announcement;

namespace WMS.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateEmployeeDto, Employee>();

        CreateMap<UpdateEmployeeDto, Employee>();

        CreateMap<CreateDepartmentDto, Department>();

        CreateMap<Department, DepartmentResponseDto>();

        CreateMap<Leave, LeaveResponseDto>()
            .ForMember(
                dest => dest.Status,
                opt => opt.MapFrom(src =>
                    src.Status.ToString()))

            .ForMember(
                dest => dest.EmpId,
                opt => opt.MapFrom(src => src.EmpId))

            .ForMember(
                dest => dest.Reason,
                opt => opt.MapFrom(src => src.Reason))

            .ForMember(
                dest => dest.AppliedOn,
                opt => opt.MapFrom(src => src.AppliedOn))

            .ForMember(
                dest => dest.ApprovedBy,
                opt => opt.MapFrom(src => src.ApprovedBy))

            .ForMember(
                dest => dest.ApprovedOn,
                opt => opt.MapFrom(src => src.ApprovedOn));

        CreateMap<Attendance, AttendanceResponseDto>()
            .ForMember(
                dest => dest.EmpId,
                opt => opt.MapFrom(src => src.EmpId))

            .ForMember(
                dest => dest.EmployeeName,
                opt => opt.MapFrom(src =>
                    src.Employee != null
                        ? src.Employee.FirstName + " " +
                        src.Employee.LastName
                        : string.Empty))

            .ForMember(
                dest => dest.WorkMode,
                opt => opt.MapFrom(src =>
                    src.WorkMode.ToString()));

        CreateMap<Employee, EmployeeResponseDto>()
            .ForMember(
                dest => dest.FirstName,
                opt => opt.MapFrom(src => src.FirstName))

            .ForMember(
                dest => dest.LastName,
                opt => opt.MapFrom(src => src.LastName))

            .ForMember(
                dest => dest.FullName,
                opt => opt.MapFrom(src =>
                    $"{src.FirstName} {src.LastName}"))

            .ForMember(
                dest => dest.GenderId,
                opt => opt.MapFrom(src => (int)src.Gender))

            .ForMember(
                dest => dest.DepartmentId,
                opt => opt.MapFrom(src => src.DepartmentId))

            .ForMember(
                dest => dest.DepartmentName,
                opt => opt.MapFrom(src =>
                    src.Department != null
                        ? src.Department.DepartmentName
                        : string.Empty))

            .ForMember(
                dest => dest.RoleId,
                opt => opt.MapFrom(src => src.RoleId))

            .ForMember(
                dest => dest.RoleName,
                opt => opt.MapFrom(src =>
                    src.Role != null
                        ? src.Role.RoleName
                        : string.Empty))

            .ForMember(
                dest => dest.Gender,
                opt => opt.MapFrom(src =>
                    src.Gender.ToString()))

            .ForMember(
                dest => dest.Status,
                opt => opt.MapFrom(src =>
                    src.Status.ToString()));

        CreateMap<CreateProjectDto, Project>();

        CreateMap<Project, ProjectResponseDto>()
            .ForMember(
                dest => dest.ClientName,
                opt => opt.MapFrom(src =>
                    src.Client != null
                        ? src.Client.ClientName
                        : string.Empty))

            .ForMember(
                dest => dest.Status,
                opt => opt.MapFrom(src =>
                    src.Status.ToString()));

        CreateMap<CreateClientDto, Client>();

        CreateMap<Client, ClientResponseDto>();

        CreateMap<
            EmployeeProjectAllocation,
            AllocationResponseDto>();

        CreateMap<
            Announcement,
            AnnouncementResponseDto>();
    }
}
