using WMS.Domain.Enums;

namespace WMS.Application.DTOs.Attendance;

public class CreateAttendanceDto
{
    public int EmpId { get; set; }

    public DateTime CheckIn { get; set; }

    public WorkMode WorkMode { get; set; }
}
