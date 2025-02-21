using Report.Constructor.DAL.MoscowVideoDb.Configuration;

namespace Report.Constructor.DAL.MoscowVideoDb.Entities;

[MoscowVideoEntity]
public sealed class CameraPersonalPosition
{
    public CameraPosition IdNavigation { get; set; } = null!;
    
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
    public bool IsPersonal { get; set; }
    
    public Camera Camera { get; set; } = null!;
    public ICollection<CameraSurveillanceObject> CameraSurveillanceObjects { get; set; } = new List<CameraSurveillanceObject>();
}