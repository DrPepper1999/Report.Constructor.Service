using Report.Constructor.DAL.MoscowVideoDb.Configuration;

namespace Report.Constructor.DAL.MoscowVideoDb.Entities;

[MoscowVideoEntity]
public sealed class CamerasMediaServerInfo
{
    public Guid CameraId { get; set; }
    public int MediaServerId { get; set; }
    public byte ChannelState { get; set; }
}