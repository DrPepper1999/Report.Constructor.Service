namespace WebServiceRestAdapter.Models.Responses;

public class GetCamerasGroupedByOperatorAndStateResponseDto
{
    public IEnumerable<CameraData> CameraDataCollection { get; set; } = Enumerable.Empty<CameraData>();
}

public class CameraData
{
    public required string UserName { get; set; }
    public required string Contract { get; set; }
    public required string CameraStatus { get; set; }
    public required string CameraType { get; set; }
    public required string InExploitation { get; set; }
    public int CamerasCount { get; set; }
}