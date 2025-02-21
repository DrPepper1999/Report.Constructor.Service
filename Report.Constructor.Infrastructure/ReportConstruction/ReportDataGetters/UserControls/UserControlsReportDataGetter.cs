using Report.Constructor.Core.Enums;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Filters;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Getters;
using Report.Constructor.Infrastructure.Models.ReportFilters;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.UserControls;

internal sealed class UserControlsReportDataGetter : IReportDataGetter
{
    public ReportType CompatibleReportType => ReportType.UserControlsReport;
    public Type CompatibleReportModelType { get; } = typeof(ReportFilterModel);

    private readonly IUsersRepository _usersRepository;
    private readonly ICamerasRepository _camerasRepository;
    private readonly IAuditRepository _auditRepository;

    public UserControlsReportDataGetter(
        IUsersRepository usersRepository, 
        ICamerasRepository camerasRepository,
        IAuditRepository auditRepository)
    {
        _usersRepository = usersRepository;
        _camerasRepository = camerasRepository;
        _auditRepository = auditRepository;
    }

    public async Task<IEnumerable<IReportRow>> GetReportRows(IReportFilter reportFilter)
    {
        var typedCommand = (ReportFilterModel)reportFilter;

        var userControls = await _auditRepository.GetUserControls(
            typedCommand.StartDateTime, typedCommand.EndDateTime);

        if (!userControls.Any())
        {
            return Enumerable.Empty<IReportRow>();
        }

        var usersIds = userControls.Select(x => x.UserId).ToList();
        var cameraIds = userControls.Select(x => x.CameraId).ToList();

        var usersData = await _usersRepository.GetUsersUserGroupData(usersIds,
            typedCommand.UserGroupsIds);

        var camerasTitles = await _camerasRepository.GetCamerasTitles(cameraIds);

        var report = userControls
            .Join(usersData, action => action.UserId, user => user.UserId, (action, user) => new { action, user })
            .Join(camerasTitles, x => x.action.CameraId, cam => cam.Id, (x, cam) => new { x.action, x.user, cam })
            .OrderByDescending(x => x.action.Date)
            .ThenBy(x => x.user.UserFullName)
            .Select(x => new UserControlsReportRow
            {
                DateString = x.action.Date.ToShortDateString(),
                UserName = x.user.UserFullName,
                UserGroupName = x.user.UserGroupName,
                ParentGroupName = x.user.ParentUserGroupName,
                ActionCount = x.action.Count,
                CameraTitle = x.cam.Title
            });

        return report;
    }
}