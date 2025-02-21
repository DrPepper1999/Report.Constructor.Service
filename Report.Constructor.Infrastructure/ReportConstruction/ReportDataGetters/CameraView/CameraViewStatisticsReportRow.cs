using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using System.ComponentModel;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.CameraView;

internal sealed class CameraViewStatisticsReportRow : IReportRow
{
    [DisplayName("Дата")]
    public DateTime Date { get; set; }
    
    [DisplayName("Действие")]
    public required string Action { get; set; }
    
    [DisplayName("ФИО")]
    public required string UserName { get; set; }
    
    [DisplayName("Группа")]
    public required string UserGroupName { get; set; }
    
    [DisplayName("Логин_AD")]
    public required string ActiveDirectoryLogin { get; set; }
    
    [DisplayName("Логин_СУДИР")]
    public required string SudirLogin { get; set; }
    
    [DisplayName("Тип")]
    public required string Type { get; set; }
    
    [DisplayName("IP")]
    public required string Ip { get; set; }
    
    [DisplayName("Портал")]
    public required string PortalType { get; set; }
}