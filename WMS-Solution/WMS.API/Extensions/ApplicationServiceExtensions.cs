using FluentValidation;
using FluentValidation.AspNetCore;
using WMS.Application.Interfaces.Services;
using WMS.Application.Mappings;
using WMS.Application.Services;
using WMS.Application.Validators.Employee;

namespace WMS.API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection
        AddApplicationServices(
            this IServiceCollection services)
    {
        services.AddAutoMapper(
            typeof(MappingProfile));

        services.AddScoped<
            IEmployeeService,
            EmployeeService>();

        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<
            IDepartmentService,
            DepartmentService>();

        services.AddScoped<
            IAttendanceService,
            AttendanceService>();

        services.AddScoped<
            ILeaveService,
            LeaveService>();

        services.AddScoped<
            IProjectService,
            ProjectService>();

        services.AddScoped<
            IClientService,
            ClientService>();

        services.AddScoped<
            IAllocationService,
            AllocationService>();

        services.AddScoped<
            IAnnouncementService,
            AnnouncementService>();

        services.AddScoped<
            IDashboardService,
            DashboardService>();

        services.AddScoped<
            IReportService,
            ReportService>();

        services.AddFluentValidationAutoValidation();

        services.AddValidatorsFromAssemblyContaining<
            CreateEmployeeDtoValidator>();

        return services;
    }
}
