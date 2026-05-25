using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces;

public interface IReportRepository
{
    Task<List<Attendance>>
        GetAttendanceReportAsync();
}
