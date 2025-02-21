using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using System.ComponentModel;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.CamerasByOperatorAndState;

internal sealed class CamerasByOperatorAndStateReportRow : IReportRow
{
    [DisplayName("Пользователь")]
    public required string UserName { get; set; }
    
    [DisplayName("Контракт")]
    public string? Contract { get; set; }
    
    [DisplayName("Статус_камеры")]
    public string? CameraStatus { get; set; }

    [DisplayName("Тип_камеры")] 
    public required string CameraType { get; set; }

    [DisplayName("В_эксплуатации")]
    public required string InExploitation { get; set; }
    
    [DisplayName("Количество_камер")]
    public int CamerasCount { get; set; }
}