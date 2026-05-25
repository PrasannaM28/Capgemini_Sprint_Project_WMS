using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces;

public interface IAttendanceRepository
    : IRepository<Attendance>
{
    Task<IEnumerable<Attendance>>
        GetMonthlyAttendanceAsync(
            int employeeId,
            int month,
            int year);
}
