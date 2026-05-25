using WMS.Application.DTOs.Dashboard;

namespace WMS.Application.Interfaces.Services;

public interface IDashboardService
{
    Task<DashboardResponseDto>
        GetDashboardAsync(
            string role,
            int? employeeId,
            int? departmentId);
}
