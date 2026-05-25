using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Persistence;

namespace WMS.Infrastructure.Repositories;

public class AttendanceRepository
    : GenericRepository<Attendance>,
      IAttendanceRepository
{
    public AttendanceRepository(
        WmsDbContext context)
        : base(context)
    {
    }

    public async Task<IEnumerable<Attendance>>
        GetMonthlyAttendanceAsync(
            int employeeId,
            int month,
            int year)
    {
        return await _context.Attendances
            .Include(a => a.Employee)
            .Where(a =>
                a.EmpId == employeeId &&
                a.AttendanceDate.Month == month &&
                a.AttendanceDate.Year == year)
            .ToListAsync();
    }
}
