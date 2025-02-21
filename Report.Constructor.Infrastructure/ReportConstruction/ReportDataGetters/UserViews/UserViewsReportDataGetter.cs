using Report.Constructor.Core.Enums;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Filters;
using Report.Constructor.Infrastructure.Models.ReportFilters;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.UserViews;

// Implementation of report "Просмотры пользователями" from report constructor. Not used in production, but may be useful  
internal sealed class UserViewsReportDataGetter
{
    public ReportType CompatibleReportType { get; }
    public Type CompatibleReportModelType { get; } = typeof(ReportFilterModel);

    private readonly IAuditRepository _auditRepository;
    private readonly IUsersRepository _usersRepository;
    public UserViewsReportDataGetter(
        IUsersRepository usersRepository, 
        IAuditRepository auditRepository)
    {
        _usersRepository = usersRepository;
        _auditRepository = auditRepository;
    }

    public async Task<IEnumerable<IReportRow>> GetReportRows(IReportFilter reportFilter)
    {
        var typedCommand = (ReportFilterModel)reportFilter;

        var userViews = await _auditRepository.GetUserViews(
            typedCommand.StartDateTime, typedCommand.EndDateTime);

        if (!userViews.Any())
        {
            return Enumerable.Empty<IReportRow>();
        }

        var usersIds = userViews.Select(x => x.UserId).ToArray();

        var usersData = await _usersRepository.GetUsersUserGroupData(usersIds,
            typedCommand.UserGroupsIds);

        var report = userViews
            .Join(usersData, action => action.UserId, user => user.UserId, 
                (action, user) => new { action, user })
            .OrderByDescending(x => x.action.Date)
            .ThenBy(x => x.user.UserFullName)
            .Select(x => new UserViewsReportRow
            {
                DateString = x.action.Date.ToShortDateString(),
                UserName = x.user.UserFullName,
                UserGroupName = x.user.UserGroupName,
                ParentGroupName = x.user.ParentUserGroupName,
                ActionCount = x.action.ViewCount
            });

        return report;
    }
}