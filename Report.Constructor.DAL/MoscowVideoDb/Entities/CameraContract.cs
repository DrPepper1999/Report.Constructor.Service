using Report.Constructor.DAL.MoscowVideoDb.Configuration;

namespace Report.Constructor.DAL.MoscowVideoDb.Entities;

[MoscowVideoEntity]
public sealed class CameraContract
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
}