using Report.Constructor.DAL.MoscowVideoDb.Configuration;

namespace Report.Constructor.DAL.MoscowVideoDb.Entities;

[MoscowVideoEntity]
public sealed class User
{
    public Guid Id { get; set; }
    public string? Login { get; set; }
    public string? LastName { get; set; }
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }

    public required UserGroup UserGroup { get; set; }
    public Guid UserGroupId { get; set; }
}