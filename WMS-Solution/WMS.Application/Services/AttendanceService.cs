using AutoMapper;
using WMS.Application.DTOs.Attendance;
using WMS.Application.Exceptions;
using WMS.Application.Interfaces.Services;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services;

public class AttendanceService
    : IAttendanceService
{
    private static readonly string[] BusinessTimeZoneIds =
    {
        "India Standard Time",
        "Asia/Kolkata"
    };

    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    public AttendanceService(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;

        _mapper = mapper;
    }

    public async Task<AttendanceResponseDto>
        CheckInAsync(CreateAttendanceDto dto)
    {
        var now = GetBusinessNow();

        var attendance = new Attendance
        {
            EmpId = dto.EmpId,

            CheckIn = now,

            AttendanceDate = now.Date,

            WorkMode = dto.WorkMode
        };

        await _unitOfWork
            .Attendances
            .AddAsync(attendance);

        await _unitOfWork
            .SaveChangesAsync();

        return _mapper.Map<
            AttendanceResponseDto>(attendance);
    }

    public async Task<AttendanceResponseDto>
        CheckOutAsync(CheckoutAttendanceDto dto)
    {
        var now = GetBusinessNow();

        var attendance =
            await _unitOfWork
                .Attendances
                .GetByIdAsync(dto.AttendanceId);

        if (attendance == null)
        {
            throw new NotFoundException(
                "Attendance not found.");
        }

        attendance.CheckOut = now;

        attendance.TotalHours =
            (attendance.CheckOut.Value - attendance.CheckIn)
            .TotalHours;

        _unitOfWork
            .Attendances
            .Update(attendance);

        await _unitOfWork
            .SaveChangesAsync();

        return _mapper.Map<
            AttendanceResponseDto>(attendance);
    }

    public async Task<AttendanceResponseDto?>
        GetAttendanceByIdAsync(int attendanceId)
    {
        var attendance = await _unitOfWork
            .Attendances
            .GetByIdAsync(attendanceId);

        return attendance == null
            ? null
            : _mapper.Map<AttendanceResponseDto>(attendance);
    }

    private static DateTime GetBusinessNow()
    {
        foreach (var timeZoneId in BusinessTimeZoneIds)
        {
            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

                return TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.UtcNow,
                    timeZone);
            }
            catch (TimeZoneNotFoundException)
            {
            }
            catch (InvalidTimeZoneException)
            {
            }
        }

        return DateTime.UtcNow;
    }

    public async Task<IEnumerable<
        AttendanceResponseDto>>
        GetMonthlyAttendanceAsync(
            int employeeId,
            int month,
            int year)
    {
        var attendance =
            await _unitOfWork
                .Attendances
                .GetMonthlyAttendanceAsync(
                    employeeId,
                    month,
                    year);

        return _mapper.Map<
            IEnumerable<AttendanceResponseDto>>
            (attendance);
    }
}
