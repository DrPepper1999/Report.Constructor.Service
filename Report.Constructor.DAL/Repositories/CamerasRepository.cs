using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.DAL.MoscowVideoDb;
using Report.Constructor.DAL.MoscowVideoDb.Entities;
using WebServiceRestAdapter;
using WebServiceRestAdapter.Models.Requests;

namespace Report.Constructor.DAL.Repositories;

internal sealed class CamerasRepository : ICamerasRepository
{
    private readonly MoscowVideoDbContext _moscowVideoDbContext;
    private readonly WebServiceRestClient _webServiceRestClient;

    public CamerasRepository(
        WebServiceRestClient webServiceRestClient,
        MoscowVideoDbContext moscowVideoDbContext)
    {
        _webServiceRestClient = webServiceRestClient;
        _moscowVideoDbContext = moscowVideoDbContext;
    }
    
    public IQueryable<Camera> GetAllFromMoscowVideo()
    {
        return _moscowVideoDbContext.Cameras.AsNoTracking();
    }

    public async Task<WebServiceRestAdapter.Models.Responses.CameraTitleData[]>
        GetCamerasTitles(IEnumerable<Guid> camerasIds)
    {
        var response = await _webServiceRestClient.GetCamerasTitles(new GetCamerasTitlesRequestDto
        {
            CamerasIds = camerasIds
        });

        return response.CamerasTitles;
    }

    public async Task<IEnumerable<WebServiceRestAdapter.Models.Responses.CameraData>> GetCamerasGroupedByOperatorAndState(IEnumerable<Guid> userGroupsIds)
    {
        var webServiceResponse = await _webServiceRestClient.GetCamerasGroupedByOperatorAndState(new GetCamerasGroupedByOperatorAndStateRequestDto
        {
            UserGroupsIds = userGroupsIds
        });

        return webServiceResponse.CameraDataCollection;
    }

    public Task<WebServiceRestAdapter.Models.Responses.CameraTitleData[]> GetAllCameraTitles()
    {
        return _moscowVideoDbContext.Cameras
            .Select(c => new WebServiceRestAdapter.Models.Responses.CameraTitleData
            {
                Id = c.Id,
                Title = c.Title
            })
            .ToArrayAsync();
    }

    public IQueryable<Guid> GetCameraIdsBySearchKey(string searchKey)
    {
        var cameras = GetAllFromMoscowVideo();

        if (string.IsNullOrWhiteSpace(searchKey))
            return cameras.Select(x => x.Id);
        
        var term = Regex.Replace(searchKey.Trim(), @"\s+", " ");
        var quotedTerm = term.Length > 9 ? $"\"{term}\"" : $"\"{term}*\"";
        
        cameras = cameras.Where(camera => EF.Functions.Contains(camera.SearchData, quotedTerm));

        return cameras.Select(x => x.Id);
    }
}