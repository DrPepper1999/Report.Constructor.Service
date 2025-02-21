using Report.Constructor.DAL.MoscowVideoDb.Configuration;

namespace Report.Constructor.DAL.MoscowVideoDb.Entities;

[MoscowVideoEntity]
public sealed class CameraCrossContractPeriod
{
    public Guid CameraId { get; set; }

    public Guid CameraContractPeriodId { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime? DateEnd { get; set; }

    public required CameraContractPeriod CameraContractPeriod { get; set; }
}