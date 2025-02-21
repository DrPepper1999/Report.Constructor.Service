using Report.Constructor.DAL.MoscowVideoDb.Enums;

namespace Report.Constructor.DAL.Interfaces.Repositories;

public interface ITagObjectsRepository
{
    IQueryable<Guid> GetTagObjectIdsAsync(IEnumerable<Guid> objectIds, IEnumerable<int> tagIds, LogicalOperatorType logicalOperatorType);
}