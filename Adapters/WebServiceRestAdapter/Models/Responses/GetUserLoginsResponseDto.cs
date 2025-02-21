namespace WebServiceRestAdapter.Models.Responses;

public sealed class GetUserLoginsResponseDto
{
    public Guid UserId { get; set; }
    public required string AdLogin { get; set; }
    public required string SudirLogin { get; set; }
}