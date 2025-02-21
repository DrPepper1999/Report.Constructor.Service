using Microsoft.EntityFrameworkCore;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.DAL.MoscowVideoDb;
using Report.Constructor.DAL.MoscowVideoDb.Dto;
using Report.Constructor.DAL.MoscowVideoDb.Entities;
using Report.Constructor.DAL.MoscowVideoDb.Enums;

namespace Report.Constructor.DAL.Repositories;

internal sealed class TokenRepository : ITokenRepository
{
    private readonly MoscowVideoDbContext _moscowVideoDbContext;

    public TokenRepository(MoscowVideoDbContext moscowVideoDbContext)
    {
        _moscowVideoDbContext = moscowVideoDbContext;
    }

    private IQueryable<Token> GetAll()
    {
        return _moscowVideoDbContext.Tokens.AsNoTracking();
    }
    
    public Task<Token[]> GetTokensAsync(
        DateTimeOffset start, DateTimeOffset end, Guid cameraId, IEnumerable<VideoAccessType> activityTypes)
    {
        return GetAll()
            .Where(t =>
                t.CreationDate >= start &&
                t.CreationDate <= end &&
                t.CameraId == cameraId &&
                t.ActivityType != null &&
                activityTypes.ToHashSet().Contains(t.ActivityType.Value))
            .ToArrayAsync();
    }
    
    public IQueryable<TokenForUserActivityWatchOutDto> GetTokensForUserActivityWatchAsync(
        DateTime start, DateTime end, Guid? userId, VideoAccessType? activityType)
    {
        return GetTokensQuery(start, end, userId, activityType)
            .Select(t => new TokenForUserActivityWatchOutDto
            {
                TokenId = t.Id,
                UserId = t.UserId,
                UserFullName = t.User.LastName + " " + t.User.FirstName + " " + t.User.MiddleName,
                UserGroupName = t.User.UserGroup.FullName ?? string.Empty,
                CameraId = t.CameraId,
                CameraTitle = t.Camera.Title,
                ActivityType = t.ActivityType
            });
    }

    private IQueryable<Token> GetTokensQuery(
        DateTime dateStart, DateTime dateEnd, Guid? userId, VideoAccessType? activityType)
    {
        var tokens = GetAll()
            .Where(t => t.CreationDate >= dateStart && t.CreationDate <= dateEnd);

        if (userId != null)
            tokens = tokens.Where(t => t.UserId == userId);

        if (activityType.HasValue)
            tokens = tokens.Where(t => t.ActivityType == activityType.Value);
        else
            tokens = tokens.Where(t => t.ActivityType == VideoAccessType.Archive
                                       || t.ActivityType == VideoAccessType.Live);
        
        return tokens;
    }
}