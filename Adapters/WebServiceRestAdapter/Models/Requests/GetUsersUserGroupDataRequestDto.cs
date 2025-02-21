namespace WebServiceRestAdapter.Models.Requests;

public class GetUsersUserGroupDataRequestDto
{
    public required IEnumerable<Guid> UsersIds { get; set; }
}