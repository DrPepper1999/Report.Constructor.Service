using Report.Constructor.Infrastructure.Models.ReportFilters;

namespace Report.Constructor.Infrastructure.Extensions;

public static class EnumerableExtensions
{
    /// <summary>
    /// Сортировка, принимающая направление сортировки в качестве параметра.
    /// </summary>
    public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        SortDirection sortDirection)
    {
        return sortDirection switch
        {
            SortDirection.Ascending => source.OrderBy(keySelector),
            SortDirection.Descending => source.OrderByDescending(keySelector),
            
            _ => throw new ArgumentOutOfRangeException(nameof(sortDirection), sortDirection, null)
        };
    }

    /// <summary>
    /// Пагинация с 1-индексацией.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Размер или номер страницы меньше 1.</exception>
    public static IEnumerable<T> Paginate<T>(this IEnumerable<T> source, Pagination? pagination)
    {
        if (pagination == null)
            return source;

        if (pagination.Size < 1)
            throw new ArgumentOutOfRangeException(nameof(Pagination.Size), pagination.Size, null);
        if (pagination.Number < 1)
            throw new ArgumentOutOfRangeException(nameof(Pagination.Number), pagination.Number, null);
        
        return source
            .Skip((pagination.Number - 1) * pagination.Size)
            .Take(pagination.Size);
    }
}