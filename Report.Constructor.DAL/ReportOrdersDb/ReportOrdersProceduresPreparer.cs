using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Report.Constructor.Core.Options;
using Report.Constructor.DAL.Interfaces;
using Report.Constructor.DAL.Synonyms;

namespace Report.Constructor.DAL.ReportOrdersDb;

internal sealed class ReportOrdersProceduresPreparer : IProceduresPreparer
{
    private readonly ReportOrdersContext _reportOrdersContext;
    private readonly DatabasesOptions _databasesOptions;

    public ReportOrdersProceduresPreparer(ReportOrdersContext reportOrdersContext,
        IOptions<DatabasesOptions> databasesOptions)
    {
        _reportOrdersContext = reportOrdersContext;
        _databasesOptions = databasesOptions.Value;
    }

    public async Task PrepareProcedures()
    {
        await AddUserDefinedTypes();

        var synonyms = GetSynonyms();
        await _reportOrdersContext.UpdateSynonyms(synonyms);

        var reportsPath = Path.Combine(AppContext.BaseDirectory, "ReportsScripts"); // This path is specified in csproj
        var reports = Directory.GetFiles(reportsPath);

        foreach (var reportPath in reports)
        {
            var reportSql = await File.ReadAllTextAsync(reportPath);
            await _reportOrdersContext.Database.ExecuteSqlRawAsync(reportSql);
        }
    }

    private async Task AddUserDefinedTypes()
    {
        await _reportOrdersContext.Database.ExecuteSqlRawAsync(
            """
            IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'ParamTableGuids')
                create type dbo.ParamTableGuids as table
                (
                    Value uniqueidentifier
                )
                
            IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'ParamTableVars')
                create type dbo.ParamTableVars as table
                (
                    Value varchar(100)
                )
                       
            IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'ParamTableOperationSharedLinkActionReportItem')
                create type dbo.ParamTableOperationSharedLinkActionReportItem as table
                (
                    UserId uniqueidentifier not null,
                    CameraId uniqueidentifier not null,
                    LinkType int not null,
                    ActionType int not null
                )
                       
            IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'ParamTableOperationScreenshotJobCrud')
            create type dbo.ParamTableOperationScreenshotJobCrud as table
            (
                UserId uniqueidentifier not null,
                JobId uniqueidentifier not null
            )
            """);
    }

    private IEnumerable<Synonym> GetSynonyms()
    {
        var archiveBalancingDbOptions = _databasesOptions.ArchiveBalancingDb;
        var aggregateTablesDbOptions = _databasesOptions.AggregateTablesDb;
        var moscowVideoDbOptions = _databasesOptions.MoscowVideoDb;

        return new Synonym[]
        {
            new("ArchiveBalancingDb_ArchiveTask", archiveBalancingDbOptions, "ArchiveTask"),
            new("AggregateTables_Cameras", aggregateTablesDbOptions, "Cameras"),
            new("AggregateTables_LiveView", aggregateTablesDbOptions, "LiveView"),
            new("AggregateTables_Users", aggregateTablesDbOptions, "Users"),
            new("AggregateTables_UserGroups", aggregateTablesDbOptions, "UserGroups"),
            new("AggregateTables_AllActive", aggregateTablesDbOptions, "AllActive"),
            new("AggregateTables_CameraActivities", aggregateTablesDbOptions, "CameraActivities"),
            new("AggregateTables_CamerasControll", aggregateTablesDbOptions, "CamerasControll"),
            new("AggregateTables_ArchiveView", aggregateTablesDbOptions, "ArchiveView"),
            new("AggregateTables_Active", aggregateTablesDbOptions, "Active"),
            new("MoscowVideo_Cameras", moscowVideoDbOptions, "Cameras"),
            new("MoscowVideo_Addresses", moscowVideoDbOptions, "Addresses"),
            new("MoscowVideo_Districts", moscowVideoDbOptions, "Districts"),
            new("MoscowVideo_Streets", moscowVideoDbOptions, "Streets"),
            new("MoscowVideo_ServiceTypes", moscowVideoDbOptions, "ServiceTypes"),
            new("MoscowVideo_ScheduleTimeLine", moscowVideoDbOptions, "ScheduleTimeLine"),
            new("MoscowVideo_ScreenshotJob", moscowVideoDbOptions, "ScreenshotJob"),
            new("MoscowVideo_Schedule", moscowVideoDbOptions, "Schedule"),
            new("MoscowVideo_Users", moscowVideoDbOptions, "Users"),
            new("MoscowVideo_UserGroups", moscowVideoDbOptions, "UserGroups"),
            new("MoscowVideo_CameraGroups", moscowVideoDbOptions, "CameraGroups"),
            new("MoscowVideo_Cameras_CameraGroups", moscowVideoDbOptions, "Cameras_CameraGroups"),
            new("MoscowVideo_ScreenshotJobResults", moscowVideoDbOptions, "ScreenshotJobResults"),
            new("MoscowVideo_CameraScheduleTasks", moscowVideoDbOptions, "CameraScheduleTasks"),
            new("MoscowVideo_CameraScheduleTaskPositions", moscowVideoDbOptions, "CameraScheduleTaskPositions"),
            new("MoscowVideo_CameraPositions_PersonalPositions", moscowVideoDbOptions, "CameraPositions_PersonalPositions"),
            new("MoscowVideo_CameraPositions", moscowVideoDbOptions, "CameraPositions"),
            new("MoscowVideo_CameraPositions_CameraSurveillanceObjects", moscowVideoDbOptions, "CameraPositions_CameraSurveillanceObjects"),
            new("MoscowVideo_CameraSurveillanceObjects", moscowVideoDbOptions, "CameraSurveillanceObjects"),
            new("MoscowVideo_StreetNames", moscowVideoDbOptions, "StreetNames"),
            new("MoscowVideo_ScreenshotJobCameraPositions", moscowVideoDbOptions, "ScreenshotJobCameraPositions"),
            new("MoscowVideo_ArchiveActionsHistories", moscowVideoDbOptions, "ArchiveActionsHistories"),
            new("MoscowVideo_ArchiveDownloadedHistory", moscowVideoDbOptions, "ArchiveDownloadedHistory"),
            new("MoscowVideo_ArchiveOrders", moscowVideoDbOptions, "ArchiveOrders"),
            new("MoscowVideo_ArchiveEntries", moscowVideoDbOptions, "ArchiveEntries"),
            new("MoscowVideo_ProblemTickets", moscowVideoDbOptions, "ProblemTickets"),
            new("MoscowVideo_ProblemReports", moscowVideoDbOptions, "ProblemReports"),
            new("MoscowVideo_Tokens", moscowVideoDbOptions, "Tokens"),
        };
    }
}