using AutoMapper;
using Moq;
using WMS.Application.DTOs.Attendance;
using WMS.Application.Mappings;
using WMS.Application.Services;
using WMS.Domain.Entities;
using WMS.Domain.Enums;
using WMS.Domain.Interfaces;

namespace WMS.Tests.Services;

public class AttendanceServiceTests
{
    private static readonly string[] BusinessTimeZoneIds =
    {
        "India Standard Time",
        "Asia/Kolkata"
    };

    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    private readonly Mock<IAttendanceRepository> _attendanceRepositoryMock;

    private readonly IMapper _mapper;

    private readonly AttendanceService _attendanceService;

    public AttendanceServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _attendanceRepositoryMock = new Mock<IAttendanceRepository>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

        _mapper = mapperConfig.CreateMapper();

        _unitOfWorkMock.Setup(x => x.Attendances)
            .Returns(_attendanceRepositoryMock.Object);

        _attendanceService = new AttendanceService(_unitOfWorkMock.Object, _mapper);
    }

    [Fact]
    public async Task CheckInAsync_ShouldStampBusinessLocalTime()
    {
        var start = GetBusinessNow();

        _attendanceRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Attendance>()))
            .Returns(Task.CompletedTask)
            .Callback<Attendance>(attendance =>
            {
                attendance.AttendanceId = 11;
                attendance.Employee = new Employee
                {
                    FirstName = "Test",
                    LastName = "User"
                };
            });

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        var result = await _attendanceService.CheckInAsync(new CreateAttendanceDto
        {
            EmpId = 1,
            WorkMode = WorkMode.WFO,
            CheckIn = DateTime.UtcNow.AddHours(-3)
        });

        var end = GetBusinessNow();

        Assert.InRange(result.CheckIn, start.AddSeconds(-2), end.AddSeconds(2));
        Assert.Equal(1, result.EmpId);
        Assert.Equal("Test User", result.EmployeeName);
    }

    [Fact]
    public async Task CheckOutAsync_ShouldUseBusinessLocalTimeAndCalculateHours()
    {
        var attendance = new Attendance
        {
            AttendanceId = 22,
            EmpId = 1,
            CheckIn = GetBusinessNow().AddHours(-2),
            AttendanceDate = GetBusinessNow().Date,
            Employee = new Employee
            {
                FirstName = "Test",
                LastName = "User"
            }
        };

        _attendanceRepositoryMock
            .Setup(x => x.GetByIdAsync(attendance.AttendanceId))
            .ReturnsAsync(attendance);

        _attendanceRepositoryMock
            .Setup(x => x.Update(It.IsAny<Attendance>()));

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        var start = GetBusinessNow();

        var result = await _attendanceService.CheckOutAsync(new CheckoutAttendanceDto
        {
            AttendanceId = attendance.AttendanceId,
            CheckOut = DateTime.UtcNow
        });

        var end = GetBusinessNow();

        Assert.InRange(result.CheckOut!.Value, start.AddSeconds(-2), end.AddSeconds(2));
        Assert.InRange(result.TotalHours, 1.9, 2.1);
        Assert.Equal(1, result.EmpId);
    }

    private static DateTime GetBusinessNow()
    {
        foreach (var timeZoneId in BusinessTimeZoneIds)
        {
            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
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
}
