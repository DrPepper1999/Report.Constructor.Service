using Microsoft.EntityFrameworkCore;
using Report.Constructor.DAL.MoscowVideoDb;
using Report.Constructor.DAL.MoscowVideoDb.Entities;
using Report.Constructor.DAL.Interfaces.Repositories;

namespace Report.Constructor.DAL.Repositories;

internal sealed class UserGroupRepository : IUserGroupRepository
{
    private readonly MoscowVideoDbContext _moscowVideoDbContext;

    public UserGroupRepository(MoscowVideoDbContext moscowVideoDbContext)
    {
        _moscowVideoDbContext = moscowVideoDbContext;
    }
    
    public Task<UserGroup[]> GetAllAsync()
    {
        return _moscowVideoDbContext.UserGroups
            .AsNoTracking()
            .ToArrayAsync();
    }
}