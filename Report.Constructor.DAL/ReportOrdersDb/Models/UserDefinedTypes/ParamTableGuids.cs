using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Report.Constructor.DAL.ReportOrdersDb.Models.UserDefinedTypes;

internal static class ParamTableGuids
{
    private const string TypeName = "[dbo].ParamTableGuids";

    public static SqlMapper.ICustomQueryParameter Create(IEnumerable<Guid>? values)
    {
        var table = new DataTable();
        table.Columns.Add("Value", typeof(Guid));

        if (values is null)
        {
            return table.AsTableValuedParameter(TypeName);
        }

        foreach (var value in values)
        {
            table.Rows.Add(value);
        }

        return table.AsTableValuedParameter(TypeName);
    }

    public static SqlParameter CreateAsSqlParameter(string parameterName, IEnumerable<Guid> values)
    {
        return CustomTableTypeParameterFactory.Create(parameterName, TypeName, values);
    }
}
