using Report.Constructor.Core.Enums;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Filters;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Getters;
using Report.Constructor.Infrastructure.Models.ReportFilters;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.UserAction;

internal sealed class UserActionReportDataGetter : IReportDataGetter
{
    public ReportType CompatibleReportType => ReportType.UserActivitiesReport;
    public Type CompatibleReportModelType { get; } = typeof(ReportFilterModel);

    private readonly IUsersRepository _usersRepository;
    private readonly IReportDataRepository _reportDataRepository;
    
    public UserActionReportDataGetter(IUsersRepository usersRepository, IReportDataRepository reportDataRepository)
    {
        _usersRepository = usersRepository;
        _reportDataRepository = reportDataRepository;
    }

    public async Task<IEnumerable<IReportRow>> GetReportRows(IReportFilter reportFilter)
    {
        var typedCommand = (ReportFilterModel)reportFilter;

        var userActions = await _reportDataRepository.GetUserActions(
            typedCommand.StartDateTime, typedCommand.EndDateTime);

        if (!userActions.Any())
        {
            return Enumerable.Empty<IReportRow>();
        }

        var usersIds = userActions.Select(ua => ua.UserId);

        var usersData = await _usersRepository.GetUsersUserGroupData(usersIds,
            typedCommand.UserGroupsIds);

        var data = userActions.Join(usersData,
            ua => ua.UserId,
            u => u.UserId,
            (action, groupData) => new UserActionReportRow
            {
                DateString = action.Date.ToShortDateString(),
                UserName = groupData.UserFullName,
                GroupName = groupData.UserGroupName,
                ParentGroupName = groupData.ParentUserGroupName,
                ActionCount = action.ActionCount
            });

        return data;
    }
}