using WMS.Domain.Enums;

namespace WMS.Application.DTOs.Leave;

public class LeaveApprovalDto
{
    public int LeaveId { get; set; }

    public LeaveStatus Status { get; set; }

    public int ApprovedBy { get; set; }
}
