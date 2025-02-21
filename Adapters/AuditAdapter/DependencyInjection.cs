using Microsoft.Extensions.DependencyInjection;
using Report.Constructor.DAL.Interfaces.Repositories;

namespace AuditAdapter;

public static class DependencyInjection
{
    public static void AddAuditAdapter(this IServiceCollection services)
    {
        services.AddTransient<IAuditRepository, AuditRepository>();
    }
}