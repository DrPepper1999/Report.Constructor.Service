using Report.Constructor.DAL.MoscowVideoDb.Configuration;
using Report.Constructor.DAL.MoscowVideoDb.Enums;

namespace Report.Constructor.DAL.MoscowVideoDb.Entities;

[MoscowVideoEntity]
public sealed class Token
{
    public Guid Id { get; set; }
    public DateTime CreationDate { get; set; }
    public Guid UserId { get; set; }
    public Guid CameraId { get; set; }
    public VideoAccessType? ActivityType { get; set; }

    public required Camera Camera { get; set; }
    public required User User { get; set; }
}