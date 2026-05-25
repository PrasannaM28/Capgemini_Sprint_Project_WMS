using Microsoft.AspNetCore.Mvc;

using WMS.Application.Interfaces.Services;

namespace WMS.API.Controllers;

[ApiController]

[Route("api/[controller]")]

public class ReportController
    : ControllerBase
{
    private readonly
        IReportService
        _reportService;

    public ReportController(
        IReportService reportService)
    {
        _reportService =
            reportService;
    }

    [HttpGet(
      "attendance")]

    public async Task<
        IActionResult>
        AttendanceReport()
    {
        var pdf =
            await _reportService
                .GenerateAttendanceReportAsync();

        return File(
            pdf,

            "application/pdf",

            "AttendanceReport.pdf");
    }
}
