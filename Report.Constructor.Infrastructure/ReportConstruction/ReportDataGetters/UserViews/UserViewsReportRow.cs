using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using System.ComponentModel;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.UserViews;

internal sealed class UserViewsReportRow : IReportRow
{
    [DisplayName("Дата")]
    public required string DateString { get; set; }

    [DisplayName("Имя_пользователя")] 
    public required string UserName { get; set; }
    
    [DisplayName("Группа")]
    public required string UserGroupName { get; set; }
    
    [DisplayName("Родительская_группа")]
    public string? ParentGroupName { get; set; }
    
    [DisplayName("Количество_действий")]
    public int ActionCount { get; set; }
}