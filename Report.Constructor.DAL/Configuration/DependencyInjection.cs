using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Report.Constructor.Core.Options;
using Report.Constructor.DAL.AggregationDb;
using Report.Constructor.DAL.ArchiveBalancingDb;
using Report.Constructor.DAL.Interfaces;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.DAL.MoscowVideoDb;
using Report.Constructor.DAL.ReportOrdersDb;
using Report.Constructor.DAL.ReportsDb;
using Report.Constructor.DAL.Repositories;

namespace Report.Constructor.DAL.Configuration;

public static class DependencyInjection
{
    public static void AddDal(this IServiceCollection services)
    {
        services.AddDbContext<ReportOrdersContext>((serviceProvider, builder) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<DatabasesOptions>>().Value;
            builder.UseSqlServer(ConstructDbConnectionString(options.ReportOrdersDb));
        });
        
        services.AddDbContext<ReportsDbContext>((serviceProvider, builder) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<DatabasesOptions>>().Value;
            builder.UseSqlServer(ConstructDbConnectionString(options.ReportsDb));
        });
        
        services.AddDbContext<AggregationDbContext>((serviceProvider, builder) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<DatabasesOptions>>().Value;
            builder.UseSqlServer(ConstructDbConnectionString(options.AggregateTablesDb));
        });
        
        services.AddDbContext<ArchiveBalancingDbContext>((serviceProvider, builder) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<DatabasesOptions>>().Value;
            builder.UseSqlServer(ConstructDbConnectionString(options.ArchiveBalancingDb));
        });
        
        services.AddDbContext<MoscowVideoDbContext>((serviceProvider, builder) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<DatabasesOptions>>().Value;
            builder.UseSqlServer(ConstructDbConnectionString(options.MoscowVideoDb), opt =>
            {
                opt.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds);
            });
        });
        
        services.AddTransient<IProceduresPreparer, ReportOrdersProceduresPreparer>();
        
        services.AddTransient<IReportOrderRepository, ReportOrderRepository>();
        services.AddTransient<IReportDataRepository, ReportDataRepository>();
        services.AddTransient<IUsersRepository, UsersRepository>();
        services.AddTransient<ICamerasRepository, CamerasRepository>();
        services.AddTransient<IArchiveTaskRepository, ArchiveTaskRepository>();
        services.AddTransient<ITokenRepository, TokenRepository>();
        services.AddTransient<IExternalServiceRepository, ExternalServiceRepository>();
        services.AddTransient<IUserGroupRepository, UserGroupRepository>();
        services.AddTransient<ICameraPersonalPositionRepository, CameraPersonalPositionRepository>();
        services.AddTransient<ITagObjectsRepository, TagObjectsRepository>();
        services.AddTransient<ITokenRepository, TokenRepository>();
    }

    private static string ConstructDbConnectionString(DatabaseOptions options)
    {
        return
            $"Server={options.Host};user id={options.UserName};password={options.Password};Database={options.Name};TrustServerCertificate=true";
    }
}