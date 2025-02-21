using Report.Constructor.DAL.MoscowVideoDb.Entities;
using Report.Constructor.DAL.AggregationDb.Models;
using Report.Constructor.DAL.ReportOrdersDb.Models;
using WebServiceRestAdapter.Models.Responses;

namespace Report.Constructor.DAL.Interfaces.Repositories;

public interface IUsersRepository
{
    Task<IEnumerable<UserUserGroupData>> GetUsersUserGroupData(IEnumerable<Guid> usersIds, Guid[] userGroupsIds);
    Task<User[]> GetAllAsync();
    Task<UserFullName[]> GetUsersFullName();
    Task<UserWithParentGroup[]> GetRecursiveUsers(IEnumerable<Guid> userGroupIds);
    Task<IReadOnlyDictionary<Guid, string>> GetUsersLoginAsync(ICollection<Guid> userIds);
    Task<Dictionary<Guid, GetUserLoginsResponseDto>> GetUsersLoginsAsync(ICollection<Guid> userIds);
}