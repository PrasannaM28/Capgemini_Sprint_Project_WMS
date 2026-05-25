using System.Security.Claims;
using WMS.Domain.Entities;
using WMS.Infrastructure.Persistence;

namespace WMS.API.Middleware;

public class AuditMiddleware
{
    private readonly RequestDelegate _next;

    public AuditMiddleware(
        RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        WmsDbContext dbContext)
    {
        var createdByClaim =
            context.User.Claims.FirstOrDefault(
                c => c.Type == ClaimTypes.NameIdentifier);

        int createdBy = 0;

        if (createdByClaim != null)
        {
            int.TryParse(
                createdByClaim.Value,
                out createdBy);
        }

        var audit = new AuditLog
        {
            EntityName =
                context.Request.Path,

            Action =
                context.Request.Method,

            RecordId = 0,

            CreatedBy = createdBy,

            CreatedOn = DateTime.UtcNow
        };

        dbContext.AuditLogs.Add(audit);

        await dbContext.SaveChangesAsync();

        await _next(context);
    }
}
