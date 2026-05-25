using Microsoft.EntityFrameworkCore;
using WMS.Domain.Enums;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Persistence;

namespace WMS.Infrastructure.Repositories;

public class DashboardRepository
    : IDashboardRepository
{
    private readonly WmsDbContext _context;

    public DashboardRepository(
        WmsDbContext context)
    {
        _context = context;
    }

    public async Task<int>
        GetTotalEmployeesAsync()
    {
        return await _context.Employees
            .CountAsync();
    }

    public async Task<int>
        GetTotalDepartmentsAsync()
    {
        return await _context.Departments
            .CountAsync();
    }

    public async Task<int>
        GetTotalProjectsAsync()
    {
        return await _context.Projects
            .CountAsync();
    }

    public async Task<int>
        GetPendingLeavesAsync()
    {
        return await _context.Leaves
            .CountAsync(l =>
                l.Status ==
                LeaveStatus.Pending);
    }

    public async Task<int>
        GetActiveAnnouncementsAsync()
    {
        return await _context.Announcements
            .CountAsync(a => a.IsActive);
    }
}
