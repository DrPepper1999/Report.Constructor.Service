using Report.Constructor.DAL.MoscowVideoDb.Dto;
using Report.Constructor.DAL.MoscowVideoDb.Entities;
using Report.Constructor.DAL.MoscowVideoDb.Enums;

namespace Report.Constructor.DAL.Interfaces.Repositories;

public interface ITokenRepository
{
    Task<Token[]> GetTokensAsync(DateTimeOffset start, DateTimeOffset end, Guid cameraId, IEnumerable<VideoAccessType> videoAccessTypes);
    IQueryable<TokenForUserActivityWatchOutDto> GetTokensForUserActivityWatchAsync(DateTime start, DateTime end, Guid? userId, VideoAccessType? activityType);
}