namespace Report.Constructor.Infrastructure.Models.ReportFilters;

public record ExtendedCameraSearchFilter
{
    public string? SearchKey { get; set; }
    public ICollection<int>? ServiceTypes { get; set; }
    public ICollection<int>? CameraModels { get; set; }
    public ICollection<int>? Districts { get; set; }
    public ICollection<int>? MunicipalDistricts { get; set; }
    public ICollection<int>? CameraStatesEnum { get; set; }
    public ICollection<Guid>? CameraContracts { get; set; }
    public ICollection<int>? SurveillanceObjects { get; set; }
    public ICollection<int>? MediaServers { get; set; }
    public ICollection<int>? Tags { get; set; }
    public bool TagsOperator { get; set; }
    public bool? IncludeCamerasWithExplotationPlacementDate { get; set; }
    public bool ShowDeleted { get; set; }
    public bool ShowOnlyRenaming { get; set; }
    public bool? ShowServiceCameras { get; set; }
    public bool? ShowExternalCameras { get; set; }
    public bool? ShowMobileCameras { get; set; }
    public bool? ShowTestCameras { get; set; }
    public bool ShowHavePtzCameras { get; set; }
    public bool HasSetPosition { get; set; }
    public bool DisableShowDeleted { get; set; }
    public bool DisableShowOnlyRenaming { get; set; }
    public bool OnlyMultiArchive { get; set; }
    public bool OnlyChannelSuspended { get; set; }
}