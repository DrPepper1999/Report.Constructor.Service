using ClosedXML.Excel;
using Microsoft.Extensions.Options;
using Report.Constructor.Core.Options;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using Report.Constructor.Infrastructure.Models.ReportConstruction;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportFileBuilders;

internal sealed class ExcelReportFileBuilder : IReportFileBuilder
{
    private const string _worksheetName = "Данные";
    private const string _fileExtension = ".xlsx";

    private readonly ReportsOptions _reportsOptions;

    public ExcelReportFileBuilder(IOptions<ReportsOptions> reportsOptions)
    {
        _reportsOptions = reportsOptions.Value;
    }

    public Task<string> BuildReportFile(ReportData reportData)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(_worksheetName);

        for (var i = 0; i < reportData.Fields.Count; i++)
        {
            var column = i + 1;
            var field = reportData.Fields.ElementAt(i);
            
            var headerCell = worksheet.Cell(1, column);
            headerCell.Value = field.Key;
            headerCell.Style.Font.SetBold();

            WriteValues(field.Value, column, worksheet);

            worksheet.Column(column).AdjustToContents();
        }

        var pathToSave = Path.Combine(_reportsOptions.ReportsPath, Guid.NewGuid() + _fileExtension);
        workbook.SaveAs(pathToSave);

        return Task.FromResult(pathToSave);
    }

    private static void WriteValues(IReadOnlyList<object?> data, int column, IXLWorksheet worksheet)
    {
        const int startRow = 2;
        for (var raw = 0; raw < data.Count; raw++)
        {
            worksheet.Cell(raw + startRow, column).Value = data[raw]?.ToString() ?? "";
        }
    }
}