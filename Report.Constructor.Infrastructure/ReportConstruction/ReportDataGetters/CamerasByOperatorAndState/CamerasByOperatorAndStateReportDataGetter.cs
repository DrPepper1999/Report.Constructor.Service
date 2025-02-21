using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Report.Constructor.Core.Enums;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Filters;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Getters;
using Report.Constructor.Infrastructure.Models.ReportFilters;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.CamerasByOperatorAndState;

internal sealed class CamerasByOperatorAndStateReportDataGetter : IReportDataGetter
{
    public ReportType CompatibleReportType => ReportType.CamerasCountByOperatorsAndStatesReport;
    public Type CompatibleReportModelType { get; } = typeof(ReportFilterModel);

    private readonly ICamerasRepository _camerasRepository;
    private readonly IMapper _mapper;

    public CamerasByOperatorAndStateReportDataGetter(IMapper mapper, ICamerasRepository camerasRepository)
    {
        _mapper = mapper;
        _camerasRepository = camerasRepository;
    }

    public async Task<IEnumerable<IReportRow>> GetReportRows(IReportFilter reportFilter)
    {
        var typedCommand = (ReportFilterModel)reportFilter;
        var now = DateTime.Now;

        var camerasData = await _camerasRepository.GetAllFromMoscowVideo()
            .GroupBy(x => new
                {
                    UserLastName = x.User.LastName,
                    UserFirstName = x.User.FirstName,
                    UserMiddleName = x.User.MiddleName,
                    Title = x.CameraCrossContractPeriods.FirstOrDefault(c => c.DateEnd == null && c.DateStart < now)
                        .CameraContractPeriod.CameraContract.Title,
                    x.CameraState.Description,
                    x.ServiceType.Name,
                    InExp = x.PlaceInExploitationDate.HasValue
                },
                camera => camera,
                (key, cam) => new CamerasByOperatorAndStateReportRow()
                {
                    UserName = key.UserLastName + " " + key.UserFirstName + " " + key.UserMiddleName,
                    Contract = key.Title,
                    CameraStatus = key.Description,
                    CameraType = key.Name,
                    InExploitation = key.InExp ? "Да" : "Нет",
                    CamerasCount = cam.Count()
                }).ToListAsync();

        return camerasData;
    }
}