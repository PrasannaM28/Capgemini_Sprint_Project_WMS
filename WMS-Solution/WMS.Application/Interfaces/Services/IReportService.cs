namespace WMS.Application.Interfaces.Services;

public interface IReportService
{
    Task<byte[]>
        GenerateAttendanceReportAsync();
}
