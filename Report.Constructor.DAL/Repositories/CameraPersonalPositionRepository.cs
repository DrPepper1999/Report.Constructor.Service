using Microsoft.EntityFrameworkCore;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.DAL.MoscowVideoDb;
using Report.Constructor.DAL.MoscowVideoDb.Entities;

namespace Report.Constructor.DAL.Repositories;

public class CameraPersonalPositionRepository : ICameraPersonalPositionRepository
{
    private readonly MoscowVideoDbContext _moscowVideoDbContext;

    public CameraPersonalPositionRepository(MoscowVideoDbContext moscowVideoDbContext)
    {
        _moscowVideoDbContext = moscowVideoDbContext;
    }

    public IQueryable<CameraPersonalPosition> GetAll()
    {
        return _moscowVideoDbContext.CameraPersonalPositions.AsNoTracking();
    }
}