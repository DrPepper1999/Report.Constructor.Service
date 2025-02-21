namespace WebServiceRestAdapter.Models.Requests;

public class GetCamerasTitlesRequestDto
{
    public required IEnumerable<Guid> CamerasIds { get; set; }
}