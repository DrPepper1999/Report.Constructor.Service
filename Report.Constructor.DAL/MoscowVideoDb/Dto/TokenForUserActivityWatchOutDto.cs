using Report.Constructor.DAL.MoscowVideoDb.Enums;

namespace Report.Constructor.DAL.MoscowVideoDb.Dto;

public record TokenForUserActivityWatchOutDto
{
    public Guid TokenId { get; set; }
    public Guid UserId { get; set; }
    public Guid CameraId { get; set; }
    public required string UserFullName { get; set; }
    public required string UserGroupName { get; set; }
    public string? CameraTitle { get; set; }
    public VideoAccessType? ActivityType { get; set; }
}