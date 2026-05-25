using WMS.API.Middleware;

namespace WMS.API.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder
        UseGlobalExceptionMiddleware(
            this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        return app;
    }

    public static IApplicationBuilder
    UseAuditMiddleware(
        this IApplicationBuilder app)
    {
        app.UseMiddleware<AuditMiddleware>();

        return app;
    }
}
