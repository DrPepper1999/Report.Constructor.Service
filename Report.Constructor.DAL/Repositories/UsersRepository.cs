using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Report.Constructor.DAL.AggregationDb;
using Report.Constructor.DAL.AggregationDb.Models;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.DAL.MoscowVideoDb;
using Report.Constructor.DAL.MoscowVideoDb.Entities;
using Report.Constructor.DAL.ReportOrdersDb.Models;
using WebServiceRestAdapter;
using WebServiceRestAdapter.Models.Requests;
using WebServiceRestAdapter.Models.Responses;

namespace Report.Constructor.DAL.Repositories;

internal sealed class UsersRepository : IUsersRepository
{
    private readonly AggregationDbContext _aggregationDbContext;
    private readonly WebServiceRestClient _webServiceRestClient;
    private readonly MoscowVideoDbContext _moscowVideoDbContext;

    public UsersRepository(
        AggregationDbContext aggregationDbContext,
        WebServiceRestClient webServiceRestClient,
        MoscowVideoDbContext moscowVideoDbContext)
    {
        _aggregationDbContext = aggregationDbContext;
        _webServiceRestClient = webServiceRestClient;
        _moscowVideoDbContext = moscowVideoDbContext;
    }

    public async Task<IEnumerable<UserUserGroupData>> GetUsersUserGroupData(IEnumerable<Guid> usersIds, Guid[] userGroupsIds)
    {
        var usersResponse = await _webServiceRestClient.GetUsersUserGroupData(new GetUsersUserGroupDataRequestDto
        {
            UsersIds = usersIds
        });

        var usersData = !userGroupsIds.Any()
            ? usersResponse.UsersUserGroupsData
            : usersResponse.UsersUserGroupsData.Where(d => userGroupsIds.Contains(d.UserGroupId));

        return usersData;
    }
    
    private IQueryable<User> GetAllQuery()
    {
        return _moscowVideoDbContext.Users.AsNoTracking();
    }

    public Task<User[]> GetAllAsync()
    {
        return GetAllQuery().ToArrayAsync();
    }
    
    public Task<UserFullName[]> GetUsersFullName()
    {
        return GetAllQuery()
            .Select(u => new UserFullName
            {
                UserId = u.Id, 
                FullName = u.LastName + " " + u.FirstName + " " + u.MiddleName
            })
            .ToArrayAsync();
    }

    public async Task<UserWithParentGroup[]> GetRecursiveUsers(IEnumerable<Guid> userGroupIds)
    {
        var shallFilterUserGroups = userGroupIds.Any() ? 1 : 0;
        var groupIds = $"'{string.Join(',', userGroupIds)}'";
        var sql =
            @$"
              DECLARE @UserGroupIds TABLE (Id uniqueidentifier);
              INSERT INTO @UserGroupIds
              VALUES ({groupIds});
              WITH C (GroupID, GroupName, ParentGroupID, ParentGroupName, Line) AS
                     (SELECT ug.Id,
                             ug.FullName,
                             ug.Id,
                             ug.FullName,
                             ROW_NUMBER() OVER (PARTITION BY ug.ParentUserGroupId ORDER BY ug.id)
                        FROM UserGroups AS ug WITH (NOLOCK)
                       WHERE (ug.Id IN (SELECT Id FROM @UserGroupIds) OR {shallFilterUserGroups} = 0)
                       UNION ALL
                      SELECT ugg.Id, ugg.FullName, ugg.ParentUserGroupId, c.GroupName, c.Line
                        FROM UserGroups AS ugg,
                             c
                       WHERE c.GroupID = ugg.ParentUserGroupId)
            SELECT c.ParentGroupName as ParentGroupName,
                   c.GroupName as GroupName,
                   u.UserId as UserId,
                   u.[Delete] as IsDeleted,
                   u.ad as Ad,
                   u.base as Base,
                   u.sudir as Sudir,
                   u.fio as Fio
              FROM Users u
              JOIN c ON c.GroupID = u.UserGroupId
            ";

        await using var connection = new SqlConnection(_aggregationDbContext.Database.GetConnectionString());
        await connection.OpenAsync();
        
        var data = await connection.QueryAsync<UserWithParentGroup>(
            sql,
            commandTimeout: 30);
        
        return data.ToArray();
    }
    
    public async Task<IReadOnlyDictionary<Guid, string>> GetUsersLoginAsync(ICollection<Guid> userIds)
    {
        var response = await _webServiceRestClient.GetUsersLogin(userIds);
        return response.UserToLogin;
    }

    public async Task<Dictionary<Guid, GetUserLoginsResponseDto>> GetUsersLoginsAsync(ICollection<Guid> userIds)
    {
        var result = await _webServiceRestClient.GetUsersLoginsAsync(userIds);
        
        return result.ToDictionary(k => k.UserId);
    }
}