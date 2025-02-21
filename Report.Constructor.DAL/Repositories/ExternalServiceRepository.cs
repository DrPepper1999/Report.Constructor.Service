using Microsoft.EntityFrameworkCore;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.DAL.MoscowVideoDb;
using Report.Constructor.DAL.MoscowVideoDb.Entities;

namespace Report.Constructor.DAL.Repositories;

internal sealed class ExternalServiceRepository : IExternalServiceRepository
{
    private readonly MoscowVideoDbContext _moscowVideoDbContext;

    public ExternalServiceRepository(MoscowVideoDbContext moscowVideoDbContext)
    {
        _moscowVideoDbContext = moscowVideoDbContext;
    }

    public Task<ExternalService[]> GetAllAsync()
    {
        return _moscowVideoDbContext.ExternalServices
            .Where(x => x.Key != Guid.Empty.ToString())
            .ToArrayAsync();
    }
}