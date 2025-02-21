using System.Net;
using Report.Constructor.Gateway.Models;

namespace Report.Constructor.Gateway.Middleware;

internal sealed class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError("Something went wrong: {Exception}", ex);
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (message, status) = exception switch
        {
            _ => ("Internal Server Error.", (int)HttpStatusCode.InternalServerError)
        };

        context.Response.StatusCode = status;

        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        await context.Response.WriteAsJsonAsync(new ErrorDetails
        {
            StatusCode = context.Response.StatusCode, 
            Message = message
        });
    }
}