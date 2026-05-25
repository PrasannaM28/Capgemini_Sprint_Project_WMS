using QuestPDF.Fluent;

using QuestPDF.Infrastructure;

using WMS.Application.Interfaces.Services;

using WMS.Domain.Interfaces;

namespace WMS.Application.Services;

public class ReportService
    : IReportService
{
    private readonly
        IUnitOfWork
        _unitOfWork;

    public ReportService(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork =
            unitOfWork;
    }

    public async Task<byte[]>
        GenerateAttendanceReportAsync()
    {
        var attendanceRecords =
            await _unitOfWork
                .Reports
                .GetAttendanceReportAsync();

        return Document.Create(
            container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header()
                        .Text(
                          "WMS Attendance Report")
                        .FontSize(22)
                        .Bold();

                    page.Content()

                        .Table(table =>
                        {
                            table.ColumnsDefinition(
                                columns =>
                                {
                                    columns.RelativeColumn();

                                    columns.RelativeColumn();

                                    columns.RelativeColumn();

                                    columns.RelativeColumn();

                                    columns.RelativeColumn();
                                });

                            table.Header(header =>
                            {
                                header.Cell()
                                    .Text("Employee");

                                header.Cell()
                                    .Text("Date");

                                header.Cell()
                                    .Text("Check In");

                                header.Cell()
                                    .Text("Check Out");

                                header.Cell()
                                    .Text("Mode");
                            });

                            foreach (
                                var item
                                in attendanceRecords)
                            {
                                table.Cell()
                                    .Text(
                                      item.Employee
                                      ?.FirstName
                                      ?? "");

                                table.Cell()
                                    .Text(
                                      item.CheckIn
                                      .Date
                                      .ToShortDateString());

                                table.Cell()
                                    .Text(
                                      item.CheckIn
                                      .ToString(
                                        "hh:mm tt"));

                                table.Cell()
                                    .Text(
                                      item.CheckOut
                                      ?.ToString(
                                        "hh:mm tt")
                                      ?? "-");

                                table.Cell()
                                    .Text(
                                      item.WorkMode
                                      .ToString());
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span(
                              "Generated On: ");

                            x.Span(
                              DateTime.Now
                              .ToString());
                        });
                });
            })

            .GeneratePdf();
    }
}
