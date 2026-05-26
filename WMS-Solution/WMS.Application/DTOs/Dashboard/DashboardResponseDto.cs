namespace WMS.Application.DTOs.Dashboard;

public class DashboardResponseDto
{
    public int TotalEmployees { get; set; }

    public int TotalDepartments { get; set; }

    public int TotalClients { get; set; }

    public int TotalProjects { get; set; }

    public int ActiveProjects { get; set; }

    public int CompletedProjects { get; set; }

    public int PresentToday { get; set; }

    public int AbsentToday { get; set; }

    public int PendingLeaves { get; set; }

    public int ApprovedLeaves { get; set; }

    public int RejectedLeaves { get; set; }

    public int ActiveAnnouncements { get; set; }

    public double TotalHours { get; set; }

    public int OwnAttendancePercent { get; set; }

    public int OwnPresentDays { get; set; }

    public int OwnAttendanceTargetDays { get; set; }

    public List<DashboardAttendancePointDto> AttendanceLast7Days { get; set; } = new();
}

public class DashboardAttendancePointDto
{
    public DateTime Date { get; set; }

    public int PresentCount { get; set; }
}
