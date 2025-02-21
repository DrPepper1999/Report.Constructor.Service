namespace WebServiceRestAdapter.Models.Responses;

public sealed class GetUsersLoginResponseDto
{
    public Dictionary<Guid, string> UserToLogin { get; init; }
}