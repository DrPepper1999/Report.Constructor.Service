namespace Report.Constructor.DAL.AggregationDb.Models;

public sealed class UserWithParentGroup
{
    public Guid UserId { get; set; }
    public string ParentGroupName { get; set; } = null!;
    public string GroupName { get; set; } = null!;
    public string? Fio { get; set; }
    public string? Ad { get; set; }
    public string? Base { get; set; }
    public string? Sudir { get; set; }
    public bool IsDeleted { get; set; }
}