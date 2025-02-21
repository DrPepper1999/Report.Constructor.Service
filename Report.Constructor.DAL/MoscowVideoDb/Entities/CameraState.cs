using Report.Constructor.DAL.MoscowVideoDb.Configuration;

namespace Report.Constructor.DAL.MoscowVideoDb.Entities;

[MoscowVideoEntity]
public class CameraState
{
    public int Id { get; set; }
    public string? Description { get; set; }
}