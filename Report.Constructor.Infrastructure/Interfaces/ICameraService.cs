using Report.Constructor.DAL.MoscowVideoDb.Dto;

namespace Report.Constructor.Infrastructure.Interfaces;

public interface ICameraService
{
    IQueryable<Guid> GetCameraIds(ExtendedCameraSearchQueryFilter filter);
}