using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using System.ComponentModel;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.UserAction;

internal sealed class UserActionReportRow : IReportRow
{
    [DisplayName("Дата")]
    public string DateString { get; set; } = default!;
    
    [DisplayName("Имя_пользователя")]
    public string UserName { get; set; } = default!;
    
    [DisplayName("Группа")]
    public string GroupName { get; set; } = default!;
    
    [DisplayName("Родительская_группа")]
    public string? ParentGroupName { get; set; }
    
    [DisplayName("Количество_действий")]
    public int ActionCount { get; set; }
}