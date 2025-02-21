using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using System.ComponentModel;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.LinksUsage;

internal sealed class LinksUsageReportRow : IReportRow
{
    [DisplayName("Тип_услуги")]
    public string ServiceType { get; set; } = string.Empty;

    [DisplayName("Подтип_услуги")]
    public string ServiceSubtype { get; set; } = string.Empty;

    [DisplayName("Камера")]
    public string Camera { get; set; } = string.Empty;

    [DisplayName("Округ")]
    public string District { get; set; } = string.Empty;

    [DisplayName("Адрес")]
    public string Address { get; set; } = string.Empty;

    [DisplayName("Сгенерировано_ссылок_на_Live")]
    public int GeneratedLiveLinks { get; set; }

    [DisplayName("Сгенерировано_ссылок_на_архив")]
    public int GeneratedArchiveLinks { get; set; }

    [DisplayName("Переходов_по_ссылкам_Live")]
    public int LiveLinkClicks { get; set; }

    [DisplayName("Переходов_по_ссылкам_архив")]
    public int ArchiveLinkClicks { get; set; }

    [DisplayName("Завершилось_авторизацией")]
    public int FinishedWithAuthorization { get; set; }
}