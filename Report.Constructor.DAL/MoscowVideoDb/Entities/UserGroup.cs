using Report.Constructor.DAL.MoscowVideoDb.Configuration;

namespace Report.Constructor.DAL.MoscowVideoDb.Entities;

[MoscowVideoEntity]
public sealed class UserGroup
{
    public Guid Id { get; set; }
    public string? FullName { get; set; }
    public Guid? ParentUserGroupId { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
}