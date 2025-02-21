using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;

namespace Report.Constructor.Infrastructure.Models.ReportConstruction;

internal sealed record PagedResult(IEnumerable<IReportRow> Items, int TotalCount);