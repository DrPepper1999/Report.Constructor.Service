using Report.Constructor.DAL.MoscowVideoDb.Entities;

namespace Report.Constructor.DAL.Interfaces.Repositories;

public interface ICameraPersonalPositionRepository
{
    IQueryable<CameraPersonalPosition> GetAll();
}