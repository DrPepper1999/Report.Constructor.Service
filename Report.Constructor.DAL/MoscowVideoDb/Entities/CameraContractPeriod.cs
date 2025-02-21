using Report.Constructor.DAL.MoscowVideoDb.Configuration;

namespace Report.Constructor.DAL.MoscowVideoDb.Entities;

[MoscowVideoEntity]
public sealed class CameraContractPeriod
{
    public Guid Id { get; set; }
    public CameraContract CameraContract { get; set; }
    
    public ICollection<CameraCrossContractPeriod> CameraCrossContractPeriods { get; set; } = new List<CameraCrossContractPeriod>();
}