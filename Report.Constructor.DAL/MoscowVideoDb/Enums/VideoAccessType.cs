using System.ComponentModel;

namespace Report.Constructor.DAL.MoscowVideoDb.Enums;

public enum VideoAccessType
{
    [Description("Трансляция")]
    Live = 0,
    [Description("Трансляция скриншот")]
    LiveSnapshot = 1,
    [Description("Архивная трансляция")]
    Archive = 2,
    [Description("Архивная трансляция скриншот")]
    ArchiveSnapshot = 3,
}