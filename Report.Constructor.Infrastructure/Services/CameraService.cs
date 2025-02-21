using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.DAL.MoscowVideoDb.Dto;
using Report.Constructor.DAL.MoscowVideoDb.Entities;
using Report.Constructor.DAL.MoscowVideoDb.Enums;
using Report.Constructor.Infrastructure.Interfaces;
using WebServiceRestAdapter.MoscowVideoRestApi;

namespace Report.Constructor.Infrastructure.Services;

public class CameraService : ICameraService
{
    private readonly ICamerasRepository _camerasRepository;
    private readonly ICameraPersonalPositionRepository _cameraPersonalPositionRepository;
    private readonly ITagObjectsRepository _tagObjectsRepository;

    public CameraService(
        ICamerasRepository camerasRepository,
        ICameraPersonalPositionRepository cameraPersonalPositionRepository,
        ITagObjectsRepository tagObjectsRepository)
    {
        _camerasRepository = camerasRepository;
        _cameraPersonalPositionRepository = cameraPersonalPositionRepository;
        _tagObjectsRepository = tagObjectsRepository;
    }

    public IQueryable<Guid> GetCameraIds(ExtendedCameraSearchQueryFilter filter)
    {
        var cameras = _camerasRepository.GetAllFromMoscowVideo();

        cameras = FilterQuery(cameras, filter.ShowDeleted, filter.SearchKey);
        cameras = ApplyFilterForCameras(cameras, filter);

        return cameras.Select(x => x.Id);
    }
    
    private IQueryable<Camera> FilterQuery(IQueryable<Camera> query, bool includeDeleted, string? searchKeys)
    {
        if (!includeDeleted)
        {
            query = query.Where(c => c.IsDeleted == false);
        }

        if (!string.IsNullOrEmpty(searchKeys))
        {
            var filteredCameras = _camerasRepository.GetCameraIdsBySearchKey(searchKeys);
            query = query.Join(filteredCameras, c => c.Id, f => f, (c, f) => c);
        }

        return query;
    }
    
    private IQueryable<Camera> ApplyFilterForCameras(IQueryable<Camera> cameras, ExtendedCameraSearchQueryFilter filters)
    {
        if (filters.IncludeCamerasWithExplotationPlacementDate.HasValue)
            cameras = filters.IncludeCamerasWithExplotationPlacementDate.Value
                ? cameras.Where(c => c.PlaceInExploitationDate != null)
                : cameras.Where(c => c.PlaceInExploitationDate == null);

        if (filters.Districts is { Count: > 0 })
            cameras = cameras.Where(c => filters.Districts.Contains(c.Address.DistrictId));

        if (filters.MunicipalDistricts is { Count: > 0 })
            cameras = cameras.Where(c => filters.MunicipalDistricts.Contains(
                c.Address.MunicipalDistrictId ?? 0));

        if (filters.CameraStatesEnum is { Count: > 0 })
            cameras = cameras.Where(c => filters.CameraStatesEnum.Contains(c.CameraStateId));

        if (filters.CameraModels is { Count: > 0 })
            cameras = cameras.Where(c => filters.CameraModels.Contains(c.CameraModelId));

        if (filters.ServiceTypes is { Count: > 0 })
            cameras = cameras.Where(c => c.SubtypeId.HasValue
                ? filters.ServiceTypes.Contains(c.SubtypeId.Value)
                : filters.ServiceTypes.Contains(c.ServiceTypeId));

        if (filters.ShowServiceCameras.HasValue)
            cameras = filters.ShowServiceCameras.Value
                ? cameras.Where(c => c.IsServiceCamera)
                : cameras.Where(c => !c.IsServiceCamera);

        if (filters.ShowExternalCameras.HasValue)
            cameras = filters.ShowExternalCameras.Value
                ? cameras.Where(c => c.IsExternalCamera)
                : cameras.Where(c => !c.IsExternalCamera);

        if (filters.ShowMobileCameras.HasValue)
            cameras = filters.ShowMobileCameras.Value
                ? cameras.Where(c => c.IsMobile)
                : cameras.Where(c => !c.IsMobile);

        if (filters.ShowTestCameras.HasValue)
            cameras = filters.ShowTestCameras.Value
                ? cameras.Where(c => c.IsTest)
                : cameras.Where(c => !c.IsTest);

        if (filters.CameraContracts is { Count: > 0 })
            cameras = FilterCamerasByContractIds(cameras, filters.CameraContracts);

        if (filters.HasSetPosition)
        {
            if (filters.SurveillanceObjects is { Count: > 0 })
            {
                var camerasPositions = _cameraPersonalPositionRepository.GetAll();
                cameras = cameras
                    .GroupJoin(
                        camerasPositions,
                        camera => camera,
                        position => position.Camera,
                        (camera, positions) => new
                        {
                            Camera = camera,
                            CameraPositions = positions
                                .Where(pos => pos.IsActive
                                              && !pos.IsPersonal
                                              && pos.CameraSurveillanceObjects
                                                  .Any(s => filters.SurveillanceObjects.Contains(s.Id)))
                        })
                    .Where(x => x.CameraPositions.Any())
                    .Select(x => x.Camera);
            }
            else
            {
                cameras = cameras.Where(c =>
                    c.CameraSurveillanceObjects.Any(
                        so => so.CameraPositions.Any(p => p.CameraId == c.Id && p.IsActive)));
            }
        }
        else if (filters.SurveillanceObjects is { Count: > 0 })
        {
            cameras = cameras.Where(
                c => c.CameraSurveillanceObjects.Any(s => filters.SurveillanceObjects.Contains(s.Id)));
        }

        if (filters.MediaServers is { Count: > 0 })
        {
            cameras = cameras.Where(c =>
                c.CamerasMediaServerInfo != null &&
                filters.MediaServers.Contains(c.CamerasMediaServerInfo.MediaServerId));
        }

        if (filters.OnlyChannelSuspended)
        {
            cameras = cameras.Where(c =>
                c.CamerasMediaServerInfo != null &&
                c.CamerasMediaServerInfo.ChannelState == (byte)CameraStatusOnMediaServerType.Suspended);
        }

        if (filters.Tags?.Any() == true)
        {
            var logicalOperatorType = filters.TagsOperator
                ? LogicalOperatorType.Or
                : LogicalOperatorType.And;

            var filteredCameras = _tagObjectsRepository.GetTagObjectIdsAsync(
                cameras.Select(x => x.Id), filters.Tags, logicalOperatorType);
            
            cameras = cameras.Join(filteredCameras, c => c.Id, ct => ct, (c, ct) => c);
        }

        if (filters.OnlyMultiArchive)
        {
            cameras = cameras.Where(c => c.HasMigratedArchives || c.HasAdditionalRecorder && c.IntegrationType == CameraIntegrationTypes.Type1);
        }

        return cameras;
    }
    
    private IQueryable<Camera> FilterCamerasByContractIds(IQueryable<Camera> cameras, ICollection<Guid> cameraContracts)
    {
        var currentDate = DateTime.Now.Date;
        return cameras
            .Where(c => c.CameraCrossContractPeriods
                .Where(cccp =>
                    cccp.DateStart.Date <= currentDate &&
                    cccp.DateEnd == null)
                .Any(cc => cameraContracts.Contains(cc.CameraContractPeriod.CameraContract.Id)));
    }
}