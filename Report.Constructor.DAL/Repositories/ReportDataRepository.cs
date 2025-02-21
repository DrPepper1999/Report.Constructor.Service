using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.DAL.ReportOrdersDb;
using Report.Constructor.DAL.ReportOrdersDb.Models.ReportItems;
using Report.Constructor.DAL.ReportOrdersDb.Models.UserDefinedTypes;
using Report.Constructor.DAL.ReportsDb;
using Report.Constructor.DAL.ReportsDb.Entities;
using System.Data;

namespace Report.Constructor.DAL.Repositories;

internal sealed class ReportDataRepository : IReportDataRepository
{
    private readonly ReportOrdersContext _reportOrdersContext;
    private readonly ReportsDbContext _reportsDbContext;

    public ReportDataRepository(ReportOrdersContext reportOrdersContext, ReportsDbContext reportsDbContext)
    {
        _reportOrdersContext = reportOrdersContext;
        _reportsDbContext = reportsDbContext;
    }

    public async Task<List<UserAction>> GetUserActions(DateTime startDateTime, DateTime endDateTime)
    {
        var userActions = await _reportsDbContext.UserActions
            .Where(x => x.Date >= startDateTime &&
                        x.Date < endDateTime)
            .ToListAsync();

        return userActions;
    }

    public async Task<IEnumerable<UsersLiveViewsReportItem>> GetUsersLiveViews(DateTime dateFrom, DateTime dateTo, ICollection<Guid> userGroupsIds)
    {
        const string sql = "exec [dbo].[UsersLiveViewsReport] @dateFrom, @dateTo, @UserGroupIds";
        var paramTableGuids = ParamTableGuids.Create(userGroupsIds);
        var parameters = new { dateFrom, dateTo, UserGroupIds = paramTableGuids };

        return await GetReportData<UsersLiveViewsReportItem>(sql, parameters);
    }

    public async Task<IEnumerable<EchdUserActionsReportItem>> GetEchdUserActions(DateTime dateFrom, DateTime dateTo, ICollection<Guid> userGroupsIds)
    {
        const string sql = "exec [dbo].[EchdUserActionsReport] @BegDate, @EndDate, @UserGroupIds";
        var paramTableGuids = ParamTableGuids.Create(userGroupsIds);
        var parameters = new { BegDate = dateFrom, EndDate = dateTo, UserGroupIds = paramTableGuids };

        return await GetReportData<EchdUserActionsReportItem>(sql, parameters);
    }

    public async Task<IEnumerable<UserGroupActionsReportItem>> GetUserGroupActions(DateTime dateFrom, DateTime dateTo, ICollection<Guid> userGroupsIds)
    {
        const string sql = "exec [dbo].[UserGroupActionsReport] @BegDate, @EndDate, @UserGroupIds";
        var paramTableGuids = ParamTableGuids.Create(userGroupsIds);
        var parameters = new
        {
            BegDate = dateFrom,
            EndDate = dateTo,
            UserGroupIds = paramTableGuids
        };

        return await GetReportData<UserGroupActionsReportItem>(sql, parameters);
    }

    public async Task<IEnumerable<CreatedScreenshotsReportItem>> GetCreatedScreenshotsData(
        IEnumerable<OperationScreenshotJobCrudReportItem> operationScreenshotJobCrudReportItems,
        DateTime dateFrom,
        DateTime dateTo,
        ICollection<Guid> userGroupsIds)
    {
        const string sql = "exec [dbo].[CreatedScreenshotsReport] @BegDate, @EndDate, @OperationScreenshotJobCrudItems, @UserGroupIds";
        
        var paramTableGuids = ParamTableGuids.Create(userGroupsIds);
        var paramTableOperationScreenshotJobCrud = ParamTableOperationScreenshotJobCrud.Create(operationScreenshotJobCrudReportItems);
        
        var parameters = new
        {
            BegDate = dateFrom,
            EndDate = dateTo,
            OperationScreenshotJobCrudItems = paramTableOperationScreenshotJobCrud,
            UserGroupIds = paramTableGuids
        };

        return await GetReportData<CreatedScreenshotsReportItem>(sql, parameters);
    }

    public async Task<IEnumerable<ArchiveDownloadsReportItem>> GetArchiveDownloads(DateTime dateFrom, DateTime dateTo, ICollection<Guid> userGroupsIds)
    {
        const string sql = "exec [dbo].[ArchiveDownloadsReport] @timestart, @timeend, @UserGroupIds";
        var paramTableGuids = ParamTableGuids.Create(userGroupsIds);
        var parameters = new { timestart = dateFrom, timeend = dateTo, UserGroupIds = paramTableGuids };

        return await GetReportData<ArchiveDownloadsReportItem>(sql, parameters);
    }

    public async Task<IEnumerable<CameraSchedulesReportItem>> GetCameraScheduleData(ICollection<Guid> userGroupsIds, DateTime dateFrom, DateTime dateTo)
    {
        const string sql = "exec [dbo].[CameraSchedulesReport] @timestart, @timeend, @UserGroupIds";
        var paramTableGuids = ParamTableGuids.Create(userGroupsIds);
        var parameters = new { timestart = dateFrom, timeend = dateTo, UserGroupIds = paramTableGuids };

        return await GetReportData<CameraSchedulesReportItem>(sql, parameters);
    }

    public async Task<IEnumerable<SchedulesStatisticsReportItem>> GetSchedulesStatistics(DateTime dateFrom, DateTime dateTo, ICollection<Guid> userGroupsIds)
    {
        const string sql = "exec [dbo].[SchedulesStatisticsReport] @BegDate, @EndDate, @UserGroupIds";
        var paramTableGuids = ParamTableGuids.Create(userGroupsIds);
        var parameters = new { BegDate = dateFrom, EndDate = dateTo, UserGroupIds = paramTableGuids };

        return await GetReportData<SchedulesStatisticsReportItem>(sql, parameters, timeout: 2400);
    }

    public Task<IEnumerable<LinksUsageReportItem>> GetLinksUsage(
        IEnumerable<OperationSharedLinkActionReportItem> operationSharedLinkActionReportItems,
        ICollection<Guid> userGroupsIds)
    {
        const string sql = "exec [dbo].[LinksUsageReport] @OperationSharedLinkActionItems, @UserGroupIds";
        
        var paramTableGuids = ParamTableGuids.Create(userGroupsIds);
        var paramTableOperationSharedLinkActionReportItems = ParamTableOperationSharedLinkActionReportItem
            .Create(operationSharedLinkActionReportItems);
        
        var parameters = new
        {
            OperationSharedLinkActionItems = paramTableOperationSharedLinkActionReportItems,
            UserGroupIds = paramTableGuids
        };

        return GetReportData<LinksUsageReportItem>(sql, parameters);
    }
    
    public async Task<int> GetExternalUserActivitiesTotalCountAsync(
        List<Guid> cameraIds,
        Guid? userId,
        string? accessType,
        DateTime start,
        DateTime end,
        List<string> externalLogins)
    {
        var sqlParameters = new[]
        {
            new SqlParameter("@UserId", userId.HasValue && userId.Value != Guid.Empty ? userId.Value : DBNull.Value),
            ParamTableGuids.CreateAsSqlParameter("@CameraIdsParam", cameraIds ?? new List<Guid>()),
            new SqlParameter("@CamsIsEmpty", cameraIds != null && cameraIds.Any()),
            ParamTableVars.CreateAsSqlParameter("@ExtLogins", externalLogins ?? new List<string>()),
            new SqlParameter("@ExLgIsEmpty", externalLogins != null && externalLogins.Any()),
            new SqlParameter("@ActionType", !string.IsNullOrEmpty(accessType) ? accessType.ToString() : DBNull.Value),
            new SqlParameter("@DateFrom", start),
            new SqlParameter("@DateTo", end),
        };

        var initialCommandTimeout = _reportsDbContext.Database.GetCommandTimeout();
        _reportsDbContext.Database.SetCommandTimeout(1200);

        var result = await _reportsDbContext.Database
            .SqlQueryRaw<int>(
                $"EXEC OperationPlayerAction_Сount {string.Join(", ", sqlParameters.Select(p => $"{p.ParameterName}"))}", 
                sqlParameters)
            .ToListAsync();
        
        _reportsDbContext.Database.SetCommandTimeout(initialCommandTimeout);
        return result.First();
    }

    private async Task<IEnumerable<T>> GetReportData<T>(
        string sql, object param, int? timeout = 1200, CommandType type = CommandType.Text)
    {
        await using var connection = new SqlConnection(_reportOrdersContext.Database.GetConnectionString());
        await connection.OpenAsync();

        var data = await connection.QueryAsync<T>(
            sql,
            param,
            commandTimeout: timeout,
            commandType: type);
        return data;
    }
}