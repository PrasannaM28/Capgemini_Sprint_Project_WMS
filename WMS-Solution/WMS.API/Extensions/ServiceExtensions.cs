using Microsoft.EntityFrameworkCore;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Persistence;
using WMS.Infrastructure.Repositories;
using WMS.Infrastructure.Identity;

namespace WMS.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<WmsDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IEmployeeRepository, EmployeeRepository>();

        services.AddScoped<
            IUserLoginRepository,
            UserLoginRepository>();

        services.AddScoped<
            IJwtTokenGenerator,
            JwtTokenGenerator>();

        services.AddScoped<
            IDepartmentRepository,
            DepartmentRepository>();

        services.AddScoped<
            IAttendanceRepository,
            AttendanceRepository>();

        services.AddScoped<
            ILeaveRepository,
            LeaveRepository>();

        services.AddScoped<
            IProjectRepository,
            ProjectRepository>();

        services.AddScoped<
            IClientRepository,
            ClientRepository>();

        services.AddScoped<
            IAllocationRepository,
            AllocationRepository>();

        services.AddScoped<
            IAnnouncementRepository,
            AnnouncementRepository>();

        services.AddScoped<
            IDashboardRepository,
            DashboardRepository>();

        services.AddScoped<
            IRoleRepository,
            RoleRepository>();

        services.AddScoped<
            IReportRepository,
            ReportRepository>();

        return services;
    }
}
