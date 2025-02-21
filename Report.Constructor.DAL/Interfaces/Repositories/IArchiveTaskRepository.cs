using Report.Constructor.DAL.ReportOrdersDb.Models;

namespace Report.Constructor.DAL.Interfaces.Repositories;

public interface IArchiveTaskRepository
{
    Task<ArchiveTaskFileSizeData[]> GetReadyFileSizeData(
        IEnumerable<Guid> archiveTaskIds, CancellationToken token = default);
}