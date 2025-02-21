using Microsoft.EntityFrameworkCore;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.DAL.MoscowVideoDb;
using Report.Constructor.DAL.MoscowVideoDb.Enums;

namespace Report.Constructor.DAL.Repositories;

internal sealed class TagObjectsRepository : ITagObjectsRepository
{
    private readonly MoscowVideoDbContext _moscowVideoDbContext;

    public TagObjectsRepository(MoscowVideoDbContext moscowVideoDbContext)
    {
        _moscowVideoDbContext = moscowVideoDbContext;
    }

    public IQueryable<Guid> GetTagObjectIdsAsync(
        IEnumerable<Guid> objectIds, IEnumerable<int> tagIds, LogicalOperatorType logicalOperatorType)
    {
        var groupedTagObjects = _moscowVideoDbContext.TagObjects.AsNoTracking()
            .Where(x => tagIds.Contains(x.TagId))
            .Where(x => objectIds.Contains(x.ObjectId))
            .GroupBy(x => x.ObjectId);

        groupedTagObjects = logicalOperatorType switch
        {
            LogicalOperatorType.Or => groupedTagObjects.Where(x => x.Any(y => tagIds.Contains(y.TagId))),
            
            LogicalOperatorType.And => tagIds.Aggregate(groupedTagObjects,
                (current, tagId) => current.Where(x => x.Any(o => o.TagId == tagId))),
            
            _ => throw new InvalidOperationException("Неизвестный тип логического оператора")
        };

        return groupedTagObjects.Select(x => x.Key);
    }
}