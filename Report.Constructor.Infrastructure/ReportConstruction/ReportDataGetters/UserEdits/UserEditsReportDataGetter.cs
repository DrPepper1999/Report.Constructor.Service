using Report.Constructor.Core.Enums;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Filters;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Getters;
using Report.Constructor.Infrastructure.Models.ReportFilters;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.UserEdits;

internal sealed class UserEditsReportDataGetter : IReportDataGetter
{
    public ReportType CompatibleReportType => ReportType.UserEditsReport;
    public Type CompatibleReportModelType { get; } = typeof(ReportFilterModel);

    private readonly IUsersRepository _usersRepository;
    private readonly IAuditRepository _auditRepository;

    public UserEditsReportDataGetter(
        IUsersRepository usersRepository,
        IAuditRepository auditRepository)
    {
        _usersRepository = usersRepository;
        _auditRepository = auditRepository;
    }

    public async Task<IEnumerable<IReportRow>> GetReportRows(IReportFilter reportFilter)
    {
        var typedCommand = (ReportFilterModel)reportFilter;

        var userEdits = await _auditRepository.GetUserEdits(
            typedCommand.StartDateTime, typedCommand.EndDateTime);

        if (!userEdits.Any()) {
            return Enumerable.Empty<IReportRow>();
        }

        var usersIds = userEdits.Select(x => x.UserId);

        var usersData = await _usersRepository.GetUsersUserGroupData(usersIds,
            typedCommand.UserGroupsIds);
        
        var report = userEdits
            .Join(usersData, 
                action => action.UserId, 
                user => user.UserId, 
                (action, user) => new { action, user })
            .OrderByDescending(au => au.action.Date)
            .ThenBy(au => au.user.UserFullName)
            .Select(au => new UserEditsReportRow
            {
                DateString = au.action.Date.ToShortDateString(),
                UserName = au.user.UserFullName,
                UserGroupName = au.user.UserGroupName,
                ActionCount = au.action.Count
            });

        return report;
    }
}