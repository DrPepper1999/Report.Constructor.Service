namespace WebServiceRestAdapter.Models.Responses;

public class GetCamerasTitlesResponseDto
{
    public required CameraTitleData[] CamerasTitles { get; set; }
}

public class CameraTitleData
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
}