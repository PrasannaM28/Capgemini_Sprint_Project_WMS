using Microsoft.EntityFrameworkCore;

using WMS.Domain.Entities;

using WMS.Domain.Interfaces;

using WMS.Infrastructure.Persistence;

namespace WMS.Infrastructure.Repositories;

public class ReportRepository
    : IReportRepository
{
    private readonly WmsDbContext
        _context;

    public ReportRepository(
        WmsDbContext context)
    {
        _context = context;
    }

    public async Task<
        List<Attendance>>
        GetAttendanceReportAsync()
    {
        return await _context
            .Attendances

            .Include(x =>
                x.Employee)

            .ToListAsync();
    }
}
