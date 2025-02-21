using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Report.Constructor.Application.Interfaces.Services;
using Report.Constructor.Core.Options;
using Report.Constructor.DAL.Interfaces;
using Report.Constructor.DAL.ReportOrdersDb;
using System.Text;

namespace Report.Constructor.Infrastructure.Services;

internal sealed class DbUpdater : IDbUpdater
{
    private readonly IProceduresPreparer _proceduresPreparer;
    private readonly ReportOrdersContext _reportOrdersContext;
    private readonly DatabasesOptions _databasesOptions;
    private readonly ILogger<DbUpdater> _logger;

    public DbUpdater(ReportOrdersContext reportOrdersContext,
        IOptions<DatabasesOptions> databasesOptions, ILogger<DbUpdater> logger, IProceduresPreparer proceduresPreparer)
    {
        _reportOrdersContext = reportOrdersContext;
        _logger = logger;
        _proceduresPreparer = proceduresPreparer;
        _databasesOptions = databasesOptions.Value;
    }

    public async Task MigrateDb()
    {
        try
        {
            await _reportOrdersContext.Database.MigrateAsync();

            await UpdateLinkedServers();

            await _proceduresPreparer.PrepareProcedures();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while migrating the database");

            throw;
        }
    }

    private async Task UpdateLinkedServers()
    {
        var optionsArray = new[]
        {
            _databasesOptions.ArchiveBalancingDb,
            _databasesOptions.AggregateTablesDb,
            _databasesOptions.MoscowVideoDb
        };

        var addLinkedServersStringBuilder = new StringBuilder();

        foreach (var databaseOptions in optionsArray)
        {
            AddLinkedServer(addLinkedServersStringBuilder, databaseOptions);
        }

        var sql = addLinkedServersStringBuilder.ToString();
        await _reportOrdersContext.Database.ExecuteSqlRawAsync(sql);
    }

    private static void AddLinkedServer(StringBuilder sb, DatabaseOptions dbOptions)
    {
        sb.AppendLine($$"""
            IF NOT EXISTS ( SELECT TOP (1) * FROM sysservers WHERE srvname = '{{dbOptions.Host}}' )
                EXEC sp_addlinkedserver N'{{dbOptions.Host}}';

            exec sp_addlinkedsrvlogin '{{dbOptions.Host}}'
                , 'FALSE', NULL, '{{dbOptions.UserName}}', '{{dbOptions.Password}}';
        """);
    }
}