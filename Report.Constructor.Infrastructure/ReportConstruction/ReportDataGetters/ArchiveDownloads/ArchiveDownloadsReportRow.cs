using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using System.ComponentModel;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.ArchiveDownloads;

internal sealed class ArchiveDownloadsReportRow : IReportRow
{
    [DisplayName("Номер_заказа")]
    public int OrderNumber { get; set; }

    [DisplayName("Имя_камеры")]
    public string CameraName { get; set; } = string.Empty;

    [DisplayName("Начало")]
    public DateTime Start { get; set; }

    [DisplayName("Конец")]
    public DateTime End { get; set; }

    [DisplayName("Объем__Гб")]
    public double? FileSizeInGb { get; set; }

    [DisplayName("ФИО")]
    public string FullName { get; set; } = string.Empty;

    [DisplayName("Действие")]
    public string Action { get; set; } = string.Empty;

    [DisplayName("Время")]
    public DateTime DateDownload { get; set; }

    [DisplayName("Основание")]
    public string Reason { get; set; } = string.Empty;
}