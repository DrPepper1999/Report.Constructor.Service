namespace Report.Constructor.Infrastructure.Models.ReportFilters;

/// <param name="Number">1-индексация</param>
public sealed record Pagination(int Size, int Number);