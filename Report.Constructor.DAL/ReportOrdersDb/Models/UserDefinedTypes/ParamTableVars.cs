using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Report.Constructor.DAL.ReportOrdersDb.Models.UserDefinedTypes;

internal static class ParamTableVars
{
    private const string TypeName = "[dbo].ParamTableVars";

    public static SqlMapper.ICustomQueryParameter Create(IEnumerable<string>? values)
    {
        var table = new DataTable();
        table.Columns.Add("Value", typeof(string));

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
    
    public static SqlParameter CreateAsSqlParameter(string parameterName, IEnumerable<string> values)
    {
        return CustomTableTypeParameterFactory.Create(parameterName, TypeName, values);
    }
}