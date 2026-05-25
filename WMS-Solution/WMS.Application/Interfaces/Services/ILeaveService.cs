using WMS.Application.DTOs.Leave;

namespace WMS.Application.Interfaces.Services;

public interface ILeaveService
{
    Task<IEnumerable<LeaveResponseDto>>
        GetAllAsync();

    Task<LeaveResponseDto?>
        GetByIdAsync(int leaveId);

    Task<LeaveResponseDto>
        ApplyLeaveAsync(CreateLeaveDto dto);

    Task<LeaveResponseDto>
        ApproveLeaveAsync(
            LeaveApprovalDto dto);

    Task CancelLeaveAsync(int leaveId);
}
