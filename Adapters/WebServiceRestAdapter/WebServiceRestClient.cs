using MapsterMapper;
using WebServiceRestAdapter.Models.Requests;
using WebServiceRestAdapter.Models.Responses;
using WebServiceRestAdapter.MoscowVideoRestApi;

namespace WebServiceRestAdapter;

public class WebServiceRestClient
{
    private readonly Client _client;
    private readonly IMapper _mapper;
    
    public WebServiceRestClient(HttpClient httpClient, IMapper mapper)
    {
        _client = new Client(httpClient);
        _mapper = mapper;
    }

    public async Task<GetUsersUserGroupDataResponseDto> GetUsersUserGroupData(GetUsersUserGroupDataRequestDto request)
    {
        var response = await _client.ApiReportsTechUsersUserGroupDataAsync(
            _mapper.Map<GetUsersUserGroupDataRequest>(request));

        return _mapper.Map<GetUsersUserGroupDataResponseDto>(response);
    }

    public async Task<GetCamerasTitlesResponseDto> GetCamerasTitles(GetCamerasTitlesRequestDto request)
    {
        var response = await _client.ApiReportsTechCamerasTitlesAsync(
            _mapper.Map<GetCamerasTitlesRequest>(request));

        return _mapper.Map<GetCamerasTitlesResponseDto>(response);
    }

    public async Task<GetCamerasGroupedByOperatorAndStateResponseDto> GetCamerasGroupedByOperatorAndState(
        GetCamerasGroupedByOperatorAndStateRequestDto request)
    {
        var response = await _client.ApiReportsTechCamerasGroupedByOperatorAndStateAsync(
            _mapper.Map<GetCamerasGroupedByOperatorAndStateRequest>(request));

        return _mapper.Map<GetCamerasGroupedByOperatorAndStateResponseDto>(response);
    }

    public Task UpdateReportState(UpdateReportStateRequestDto request)
    {
        return _client.ApiReportsTechUpdateReportStateAsync(
            _mapper.Map<UpdateReportStateRequest>(request));
    }

    public async Task<GetUsersLoginResponseDto> GetUsersLogin(ICollection<Guid> userIds)
    {
        var loginTasks = userIds
            .Chunk(5000)
            .Select(async userIdsChunk =>
                await _client.ApiReportsTechUsersLoginAsync(
                    new GetUsersLoginRequest { UserIds = userIdsChunk }));

        var response = await Task.WhenAll(loginTasks);
        
        return new GetUsersLoginResponseDto
        {
            UserToLogin = response
                .SelectMany(x => x.UsersLogin)
                .ToDictionary(k => k.UserId, v => v.Login)
        };
    }
    
    public async Task<ICollection<GetUserLoginsResponseDto>> GetUsersLoginsAsync(ICollection<Guid> userIds)
    {
        var loginTasks = userIds
            .Distinct()
            .Chunk(5000)
            .Select(async userIdsChunk =>
                await _client.ApiReportsTechUsersLoginsAsync(new GetUsersLoginsRequest
                {
                    UserIds = userIdsChunk
                }));

        var result = await Task.WhenAll(loginTasks);

        return _mapper.Map<ICollection<GetUserLoginsResponseDto>>(
            result.SelectMany(x => x.UsersLogins));
    }
}