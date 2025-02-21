using Report.Constructor.DAL.MoscowVideoDb.Configuration;
using Report.Constructor.DAL.MoscowVideoDb.Enums;

namespace Report.Constructor.DAL.MoscowVideoDb.Entities;

[MoscowVideoEntity]
public sealed class Camera
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public required string SearchData { get; set; }
    public required bool IsDeleted { get; set; }
    public DateTime? PlaceInExploitationDate { get; set; }
    public int CameraStateId { get; set; }
    public int CameraModelId { get; set; }
    public int? SubtypeId { get; set; }
    public int ServiceTypeId { get; set; }
    public bool IsServiceCamera { get; set; }
    public bool IsExternalCamera { get; set; }
    public bool IsMobile { get; set; }
    public bool IsTest { get; set; }
    public bool HasAdditionalRecorder { get; set; }
    public bool HasMigratedArchives { get; set; }
    public CameraIntegrationTypes IntegrationType { get; set; }

    public User? User { get; set; }
    public required Address Address { get; set; }
    public CamerasMediaServerInfo? CamerasMediaServerInfo { get; set; }
    public CameraState CameraState { get; set; }
    public ServiceType ServiceType { get; set; }
    public ICollection<Token> Tokens { get; set; } = new List<Token>();
    public ICollection<CameraCrossContractPeriod> CameraCrossContractPeriods { get; set; } = new List<CameraCrossContractPeriod>();
    public ICollection<CameraSurveillanceObject> CameraSurveillanceObjects { get; set; } = new List<CameraSurveillanceObject>();
}