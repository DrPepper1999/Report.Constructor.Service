using System.Data;
using Dapper;
using Report.Constructor.DAL.ReportOrdersDb.Models.ReportItems;

namespace Report.Constructor.DAL.ReportOrdersDb.Models.UserDefinedTypes;

internal static class ParamTableOperationSharedLinkActionReportItem
{
    public static SqlMapper.ICustomQueryParameter Create(
        IEnumerable<OperationSharedLinkActionReportItem>? operationSharedLinkActionReportItems)
    {
        var table = new DataTable();
        table.Columns.Add("UserId", typeof(Guid));
        table.Columns.Add("CameraId", typeof(Guid));
        table.Columns.Add("LinkType", typeof(int));
        table.Columns.Add("ActionType", typeof(int));

        if (operationSharedLinkActionReportItems is null)
        {
            return table.AsTableValuedParameter("[dbo].ParamTableOperationSharedLinkActionReportItem");
        }
        
        foreach (var value in operationSharedLinkActionReportItems)
        {
            table.Rows.Add(value.UserId, value.CameraId, value.LinkType, value.ActionType);
        }

        return table.AsTableValuedParameter("[dbo].ParamTableOperationSharedLinkActionReportItem");
    }
}