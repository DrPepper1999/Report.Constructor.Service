using Report.Constructor.Gateway.Middleware;
using Serilog;

namespace Report.Constructor.Gateway.Configuration;

internal static class MiddlewareConfiguration
{
    public static void ConfigureMiddleware(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.MapEndpoints();

        app.UseSerilogRequestLogging();
        
        app.UseMiddleware<ExceptionMiddleware>(); 
    }
}