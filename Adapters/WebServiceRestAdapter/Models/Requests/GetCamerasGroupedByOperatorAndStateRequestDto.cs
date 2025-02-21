namespace WebServiceRestAdapter.Models.Requests;

public class GetCamerasGroupedByOperatorAndStateRequestDto
{
    public IEnumerable<Guid> UserGroupsIds { get; set; } = Enumerable.Empty<Guid>();
}