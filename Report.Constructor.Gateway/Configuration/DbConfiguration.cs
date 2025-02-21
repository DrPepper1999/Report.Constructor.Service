using Report.Constructor.Application.Interfaces.Services;

namespace Report.Constructor.Gateway.Configuration;

internal static class DbConfiguration
{
    public static async Task MigrateDb(this WebApplication app)
    {
        var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
        using var scope = scopeFactory.CreateScope();

        var services = scope.ServiceProvider; 
        
        var dbUpdater = services.GetRequiredService<IDbUpdater>();
        await dbUpdater.MigrateDb();
    }
}