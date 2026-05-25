namespace WMS.Application.DTOs.Reports;

public class AttendanceReportDto
{
    public string EmployeeName
    { get; set; }
        = string.Empty;

    public DateTime Date
    { get; set; }

    public DateTime CheckIn
    { get; set; }

    public DateTime? CheckOut
    { get; set; }

    public string WorkMode
    { get; set; }
        = string.Empty;
}
