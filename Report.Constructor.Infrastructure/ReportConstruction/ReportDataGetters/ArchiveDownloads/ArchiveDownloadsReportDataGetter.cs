using MapsterMapper;
using Report.Constructor.Core.Enums;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Filters;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Getters;
using Report.Constructor.Infrastructure.Models.ReportFilters;
using Type = System.Type;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.ArchiveDownloads;

internal sealed class ArchiveDownloadsReportDataGetter : IReportDataGetter
{
    public ReportType CompatibleReportType => ReportType.ArchiveDownloads;
    public Type CompatibleReportModelType { get; } = typeof(ReportFilterModel);

    private readonly IArchiveTaskRepository _archiveTaskRepository;
    private readonly ICamerasRepository _camerasRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IReportDataRepository _reportDataRepository;
    private readonly IAuditRepository _auditRepository;
    private readonly IMapper _mapper;

    public ArchiveDownloadsReportDataGetter(
        IArchiveTaskRepository archiveTaskRepository,
        ICamerasRepository camerasRepository,
        IUsersRepository usersRepository,
        IReportDataRepository reportDataRepository, 
        IAuditRepository auditRepository,
        IMapper mapper)
    {
        _archiveTaskRepository = archiveTaskRepository;
        _camerasRepository = camerasRepository;
        _usersRepository = usersRepository;
        _reportDataRepository = reportDataRepository;
        _auditRepository = auditRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<IReportRow>> GetReportRows(IReportFilter reportFilter)
    {
        var typedCommand = (ReportFilterModel)reportFilter;

        var userGroupIds = typedCommand.UserGroupsIds.ToHashSet();
        
        var items = await _reportDataRepository.GetArchiveDownloads(
            typedCommand.StartDateTime, typedCommand.EndDateTime, userGroupIds);

        var archiveRestartRows = await GetArchiveRestartReportRows(
            typedCommand.StartDateTime, typedCommand.EndDateTime, userGroupIds);
         
        var rows = _mapper
            .Map<IEnumerable<ArchiveDownloadsReportRow>>(items)
            .Concat(archiveRestartRows);
        
        return rows;
    }

    private async Task<IEnumerable<ArchiveDownloadsReportRow>> GetArchiveRestartReportRows(
        DateTime startDateTime, DateTime endDateTime, IReadOnlySet<Guid> userGroupIds)
    {
        var operationOrderArchiveItems = await _auditRepository
            .GetOperationOrderArchiveData(startDateTime, endDateTime);
        
        var cameraTitles = await _camerasRepository.GetAllCameraTitles();
        var userFios = await _usersRepository.GetUsersFullName();
        var archiveTaskFileSizes = await _archiveTaskRepository
            .GetReadyFileSizeData(operationOrderArchiveItems.Select(x => x.ArchiveTaskId));

        return
            from operationOrderArchiveItem in operationOrderArchiveItems
            join cameraTitleData in cameraTitles on operationOrderArchiveItem.CameraId equals cameraTitleData.Id
            join userFio in userFios on operationOrderArchiveItem.UserId equals userFio.UserId
            join archiveTaskFileSize in archiveTaskFileSizes on operationOrderArchiveItem.ArchiveTaskId equals archiveTaskFileSize.ArchiveTaskId
            where userGroupIds.Count == 0 || userGroupIds.Contains(operationOrderArchiveItem.UserId)
            select new ArchiveDownloadsReportRow
            {
                OrderNumber = operationOrderArchiveItem.ArchiveNumber,
                CameraName = cameraTitleData.Title,
                Start = operationOrderArchiveItem.BeginTime.DateTime,
                End = operationOrderArchiveItem.EndTime.DateTime,
                FileSizeInGb = Math.Round((double)archiveTaskFileSize.FileSize / 1024, 2),
                FullName = userFio.FullName,
                Action = "Перезапуск заказа",
                DateDownload = operationOrderArchiveItem.Timestamp.DateTime,
                Reason = operationOrderArchiveItem.Reason
            };
    }
}