using Report.Constructor.Core.Enums;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Filters;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Getters;
using Report.Constructor.Infrastructure.Models.ReportFilters;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.UserGroupViews;

internal sealed class UserGroupViewsReportDataGetter : IReportDataGetter
{
    public ReportType CompatibleReportType => ReportType.UserViewsGroupedByGroupsReport;
    public Type CompatibleReportModelType { get; } = typeof(ReportFilterModel);

    private readonly IAuditRepository _auditRepository;
    private readonly IUsersRepository _usersRepository;

    public UserGroupViewsReportDataGetter(IUsersRepository usersRepository, IAuditRepository auditRepository)
    {
        _usersRepository = usersRepository;
        _auditRepository = auditRepository;
    }

    public async Task<IEnumerable<IReportRow>> GetReportRows(IReportFilter reportFilter)
    {
        var typedCommand = (ReportFilterModel)reportFilter;

        var userViews = await _auditRepository.GetUserGroupViews(
            typedCommand.StartDateTime, typedCommand.EndDateTime);

        if (!userViews.Any())
        {
            return Enumerable.Empty<IReportRow>();
        }

        var usersIds = userViews.Select(x => x.UserId);

        var usersData = await _usersRepository.GetUsersUserGroupData(usersIds,
            typedCommand.UserGroupsIds);
        
        var report = userViews
            .Join(usersData, uv => uv.UserId, u => u.UserId, (uv, u) => new { uv, u })
            .GroupBy(x => new { x.u.UserGroupName, x.u.ParentUserGroupName, x.u.UserFullName, x.u.UserId, x.uv.Date })
            .OrderByDescending(x => x.Key.Date)
            .ThenBy(x => x.Key.UserFullName)
            .Select(x => new UserGroupViewsReportRow
            {
                DateString = x.Key.Date.ToShortDateString(),
                UserName = x.Key.UserFullName,
                UserGroupName = x.Key.UserGroupName,
                ParentGroupName = x.Key.ParentUserGroupName,
                ActionCount = x.Sum(y => y.uv.ViewCount)
            });

        return report;
    }
}