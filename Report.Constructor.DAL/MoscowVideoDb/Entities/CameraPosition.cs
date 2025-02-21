using Report.Constructor.DAL.MoscowVideoDb.Configuration;

namespace Report.Constructor.DAL.MoscowVideoDb.Entities;

[MoscowVideoEntity]
public sealed class CameraPosition
{
    public Guid Id { get; set; }
    public Guid CameraId { get; set; }
    public bool IsActive { get; set; }
    
    public CameraPersonalPosition CameraPositionsPersonalPosition { get; set; }
}