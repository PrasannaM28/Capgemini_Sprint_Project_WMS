namespace WMS.Application.DTOs.Leave;

public class CreateLeaveDto
{
    public int EmpId { get; set; }

    public string LeaveType { get; set; }
        = string.Empty;

    public string? Reason { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }
}
