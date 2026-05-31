using WMS.Application.DTOs.Attendance;

namespace WMS.Application.Interfaces.Services;

public interface IAttendanceService
{
    Task<AttendanceResponseDto>
        CheckInAsync(CreateAttendanceDto dto);

    Task<AttendanceResponseDto>
        CheckOutAsync(CheckoutAttendanceDto dto);

    Task<AttendanceResponseDto?>
        GetAttendanceByIdAsync(int attendanceId);

    Task<IEnumerable<AttendanceResponseDto>>
        GetMonthlyAttendanceAsync(
            int employeeId,
            int month,
            int year);
}
