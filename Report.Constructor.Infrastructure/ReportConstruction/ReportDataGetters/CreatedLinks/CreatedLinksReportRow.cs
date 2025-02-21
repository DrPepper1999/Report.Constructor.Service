using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using System.ComponentModel;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.CreatedLinks;

internal sealed class CreatedLinksReportRow : IReportRow
{
    [DisplayName("Родительская_группа")]
    public string ParentGroup { get; set; } = string.Empty;

    [DisplayName("Группа")]
    public string Group { get; set; } = string.Empty;

    [DisplayName("Статус")]
    public string Status { get; set; } = string.Empty;

    [DisplayName("Логин_AD")]
    public string ADLogin { get; set; } = string.Empty;

    [DisplayName("Логин_базовый")]
    public string BasicLogin { get; set; } = string.Empty;

    [DisplayName("Логин_СУДИР")]
    public string SudirLogin { get; set; } = string.Empty;

    [DisplayName("ФИО")]
    public string FullName { get; set; } = string.Empty;

    [DisplayName("Кол_во_сгенерированных_ссылок_на_live")]
    public long GeneratedLiveLinksCount { get; set; }

    [DisplayName("Кол_во_сгенерированных_ссылок_на_АРХИВ")]
    public long GeneratedArchiveLinksCount { get; set; }

    [DisplayName("Переходов_по_созданным_ссылкам_Live")]
    public long LiveLinkClicks { get; set; }

    [DisplayName("Переходов_по_созданным_ссылкам_АРХИВ")]
    public long ArchiveLinkClicks { get; set; }

    [DisplayName("Завершилось_авторизацией")]
    public long AuthenticatedLinkClicks { get; set; }

    [DisplayName("Кол_во_уникальных_камер")]
    public long UniqueCamerasCount { get; set; }
}