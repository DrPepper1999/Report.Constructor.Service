using Report.Constructor.DAL.MoscowVideoDb.Entities;
using WebServiceRestAdapter.Models.Responses;

namespace Report.Constructor.DAL.Interfaces.Repositories;

public interface ICamerasRepository
{
    IQueryable<Camera> GetAllFromMoscowVideo();

    Task<CameraTitleData[]> GetCamerasTitles(IEnumerable<Guid> camerasIds);
    Task<IEnumerable<CameraData>> GetCamerasGroupedByOperatorAndState(IEnumerable<Guid> userGroupsIds);
    Task<CameraTitleData[]> GetAllCameraTitles();
    IQueryable<Guid> GetCameraIdsBySearchKey(string searchKey);
}