using System.ComponentModel;

namespace Report.Constructor.Infrastructure.Models.ReportConstruction;

internal sealed class ReportData
{
    public Dictionary<string, List<object?>> Fields { get; set; } = new();
    
    internal static ReportData CreateReportData<T>(ICollection<T> collection) where T : class
    {
        var reportData = new ReportData();

        if (!collection.Any())
        {
            return reportData;
        }
        
        var properties = collection.First().GetType().GetProperties();

        var displayNames = properties.ToDictionary(p => p.Name, p =>
            p.GetCustomAttributes(typeof(DisplayNameAttribute), false)
                .Cast<DisplayNameAttribute>()
                .FirstOrDefault()?.DisplayName ?? p.Name);

        foreach (var item in collection)
        {
            foreach (var property in properties)
            {
                var displayName = displayNames[property.Name];

                if (!reportData.Fields.TryGetValue(displayName, out var fieldData))
                {
                    fieldData = new List<object?>();
                    reportData.Fields[displayName] = fieldData;
                }

                var value = property.GetValue(item);
                fieldData.Add(value);
            }
        }

        return reportData;
    }
}