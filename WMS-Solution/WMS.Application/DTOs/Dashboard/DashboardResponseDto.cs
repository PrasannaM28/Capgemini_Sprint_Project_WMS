namespace WMS.Application.DTOs.Dashboard;

public class DashboardResponseDto
{
    public int TotalEmployees { get; set; }

    public int TotalDepartments { get; set; }

    public int TotalProjects { get; set; }

    public int PendingLeaves { get; set; }

    public int ActiveAnnouncements { get; set; }

    public double TotalHours { get; set; }
}
