using Microsoft.EntityFrameworkCore;
using Report.Constructor.DAL.ArchiveBalancingDb;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.DAL.ReportOrdersDb.Models;

namespace Report.Constructor.DAL.Repositories;

public class ArchiveTaskRepository : IArchiveTaskRepository
{
    private readonly ArchiveBalancingDbContext _archiveBalancingDbContext;

    public ArchiveTaskRepository(ArchiveBalancingDbContext archiveBalancingDbContext)
    {
        _archiveBalancingDbContext = archiveBalancingDbContext;
    }

    public Task<ArchiveTaskFileSizeData[]> GetReadyFileSizeData(IEnumerable<Guid> archiveTaskIds, CancellationToken token)
    {
        return _archiveBalancingDbContext.ArchiveTasks
            .AsNoTracking()
            .Where(at => at.FileSize.HasValue && archiveTaskIds.Contains(at.Id))
            .Select(at => new ArchiveTaskFileSizeData
            {
                ArchiveTaskId = at.Id,
                FileSize = at.FileSize!.Value
            })
            .ToArrayAsync(token);
    }
}