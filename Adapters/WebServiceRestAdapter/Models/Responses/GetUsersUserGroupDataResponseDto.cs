namespace WebServiceRestAdapter.Models.Responses;

public class GetUsersUserGroupDataResponseDto
{
    public required UserUserGroupData[] UsersUserGroupsData { get; set; }
}

public class UserUserGroupData
{
    public Guid UserId { get; set; }
    public string UserFullName { get; set; } = default!;
    public string UserGroupName { get; set; } = default!;
    public Guid UserGroupId { get; set; }
    public string? ParentUserGroupName { get; set; }
}