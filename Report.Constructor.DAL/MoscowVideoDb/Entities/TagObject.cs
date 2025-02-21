using Report.Constructor.DAL.MoscowVideoDb.Configuration;

namespace Report.Constructor.DAL.MoscowVideoDb.Entities;

[MoscowVideoEntity]
public sealed class TagObject
{
    public int Id { get; set; }
    public Guid ObjectId { get; set; }
    public int TagId { get; set; }
}