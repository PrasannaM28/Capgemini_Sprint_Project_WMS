namespace WMS.Domain.Interfaces;

public interface IDashboardRepository
{
    Task<int> GetTotalEmployeesAsync();

    Task<int> GetTotalDepartmentsAsync();

    Task<int> GetTotalProjectsAsync();

    Task<int> GetPendingLeavesAsync();

    Task<int> GetActiveAnnouncementsAsync();
}
