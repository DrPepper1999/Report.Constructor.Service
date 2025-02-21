using Report.Constructor.Core.Enums;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Filters;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Getters;
using Report.Constructor.Infrastructure.Models.ReportFilters;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.UserGroupControl;

internal sealed class UserGroupControlReportDataGetter : IReportDataGetter
{
    public ReportType CompatibleReportType => ReportType.UserManagementsGroupedByGroupsReport;
    public Type CompatibleReportModelType { get; } = typeof(ReportFilterModel);

    private readonly IUsersRepository _usersRepository;
    private readonly IAuditRepository _auditRepository;

    public UserGroupControlReportDataGetter(IUsersRepository usersRepository, IAuditRepository auditRepository)
    {
        _usersRepository = usersRepository;
        _auditRepository = auditRepository;
    }

    public async Task<IEnumerable<IReportRow>> GetReportRows(IReportFilter reportFilter)
    {
        var typedCommand = (ReportFilterModel)reportFilter;

        var userControls = await _auditRepository.GetUserGroupControls(
            typedCommand.StartDateTime, typedCommand.EndDateTime);

        if (!userControls.Any())
        {
            return Enumerable.Empty<IReportRow>();
        }

        var usersIds = userControls.Select(x => x.UserId);

        var usersData = await _usersRepository.GetUsersUserGroupData(usersIds,
            typedCommand.UserGroupsIds);

        var report = userControls
            .Join(usersData, uc => uc.UserId, user => user.UserId, (uc, user) => new { uc, user })
            .GroupBy(x => new { x.user.UserGroupName, x.uc.Date })
            .Select(g => new UserGroupControlReportRow
            {
                DateString = g.Key.Date.ToShortDateString(),
                UserGroupName = g.Key.UserGroupName,
                ActionCount = g.Sum(x => x.uc.Count)
            })
            .OrderByDescending(x => x.DateString)
            .ThenBy(x => x.UserGroupName)
            .ToList();

        return report;
    }
}