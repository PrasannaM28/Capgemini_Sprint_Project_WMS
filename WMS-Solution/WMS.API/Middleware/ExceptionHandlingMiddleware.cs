using System.Net;
using System.Text.Json;
using FluentValidation;
using WMS.Application.Common;
using WMS.Application.Exceptions;

namespace WMS.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;

        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, exception.Message);

            await HandleExceptionAsync(context, exception);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ApiResponse<object>();

        switch (exception)
        {
            case ValidationException validationException:

                context.Response.StatusCode =
                    (int)HttpStatusCode.BadRequest;

                response.Success = false;

                response.Message = "Validation failed.";

                response.Errors = validationException.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList();

                break;

            case NotFoundException:

                context.Response.StatusCode =
                    (int)HttpStatusCode.NotFound;

                response.Success = false;

                response.Message = exception.Message;

                break;

            case BadRequestException:

                context.Response.StatusCode =
                    (int)HttpStatusCode.BadRequest;

                response.Success = false;

                response.Message = exception.Message;

                break;

            default:

                context.Response.StatusCode =
                    (int)HttpStatusCode.InternalServerError;

                response.Success = false;

                response.Message =
                    "An unexpected error occurred.";

                break;
        }

        var jsonResponse = JsonSerializer.Serialize(response);

        await context.Response.WriteAsync(jsonResponse);
    }
}
