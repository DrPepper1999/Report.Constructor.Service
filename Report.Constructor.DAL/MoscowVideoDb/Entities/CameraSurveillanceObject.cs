using Report.Constructor.DAL.MoscowVideoDb.Configuration;

namespace Report.Constructor.DAL.MoscowVideoDb.Entities;

[MoscowVideoEntity]
public sealed class CameraSurveillanceObject
{
    public int Id { get; set; }
    public ICollection<CameraPosition> CameraPositions { get; set; } = new List<CameraPosition>();
    public ICollection<Camera> Cameras { get; set; } = new List<Camera>();
}