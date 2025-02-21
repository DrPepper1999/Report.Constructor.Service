using Report.Constructor.Core.Enums;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Filters;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction.Getters;
using Report.Constructor.Infrastructure.Models.ReportFilters;

namespace Report.Constructor.Infrastructure.ReportConstruction.ReportDataGetters.CreatedLinks;

internal sealed class CreatedLinksReportDataGetter : IReportDataGetter
{
    public ReportType CompatibleReportType => ReportType.CreatedLinks;
    public Type CompatibleReportModelType { get; } = typeof(ReportFilterModel);

    private readonly IUsersRepository _usersRepository;
    private readonly IAuditRepository _auditRepository;

    public CreatedLinksReportDataGetter(IUsersRepository usersRepository, IAuditRepository auditRepository)
    {
        _usersRepository = usersRepository;
        _auditRepository = auditRepository;
    }

    public async Task<IEnumerable<IReportRow>> GetReportRows(IReportFilter reportFilter)
    {
        var typedCommand = (ReportFilterModel)reportFilter;

        var items = await _auditRepository.GetOperationSharedLinkActionData(
            typedCommand.StartDateTime, typedCommand.EndDateTime);

        var users = await _usersRepository.GetRecursiveUsers(typedCommand.UserGroupsIds);
        
        var uniqueCamerasCount = items
            .Where(x => x.ActionType == 1)
            .Select(x => new {x.UserId, x.CameraId})
            .Distinct()
            .GroupBy(x => x.UserId)
            .Select(g => new CountByUserModel(g.Key, g.Count()));

        var generatedLiveLinksCount = items
            .Where(x => x is
            {
                LinkType: 1,
                ActionType: 1 // пользователь создал линк на Live
            })
            .GroupBy(x => x.UserId, x => x.Id)
            .Select(g => new CountByUserModel(g.Key, g.Distinct().Count()));
        
        var generatedArchiveLinksCount = items
            .Where(x => x is
            {
                LinkType: 2,
                ActionType: 1 // пользователь создал линк на Архив
            })
            .GroupBy(x => x.UserId, x => x.Id)
            .Select(g => new CountByUserModel(g.Key, g.Distinct().Count()));
            
        var liveLinkViewsCount = items
            .Where(x => x is
            {
                LinkType: 1,
                ActionType: 2 // просмотренно ссылок Live
            })
            .GroupBy(x => x.UserId)
            .Select(g => new CountByUserModel(g.Key, g.Count()));
            
        var archiveLinkViewsCount = items
            .Where(x => x is
            {
                LinkType: 2,
                ActionType: 2 // просмотренно ссылок Архив
            })
            .GroupBy(x => x.UserId)
            .Select(g => new CountByUserModel(g.Key, g.Count()));
        
        var authenticatedLinkClicks = items
            .Where(x => x.ActionType == 3) // Завершилось авторизацией
            .GroupBy(x => x.UserId)
            .Select(g => new CountByUserModel(g.Key, g.Count()));

        return from user in users
            join ucc in uniqueCamerasCount on user.UserId equals ucc.UserId into uccGroup
            join gllc in generatedLiveLinksCount on user.UserId equals gllc.UserId into gllcGroup
            join galc in generatedArchiveLinksCount on user.UserId equals galc.UserId into galcGroup
            join llvc in liveLinkViewsCount on user.UserId equals llvc.UserId into llvcGroup
            join alvc in archiveLinkViewsCount on user.UserId equals alvc.UserId into alvcGroup
            join alc in authenticatedLinkClicks on user.UserId equals alc.UserId into alcGroup
            from ucc in uccGroup.DefaultIfEmpty()
            from gllc in gllcGroup.DefaultIfEmpty()
            from galc in galcGroup.DefaultIfEmpty()
            from llvc in llvcGroup.DefaultIfEmpty()
            from alvc in alvcGroup.DefaultIfEmpty()
            from alc in alcGroup.DefaultIfEmpty()
            select new CreatedLinksReportRow
            {
                ParentGroup = user.ParentGroupName,
                Group = user.GroupName,
                Status = user.IsDeleted ? "Deleted" : "Active",
                ADLogin = user.Ad,
                BasicLogin = user.Base,
                SudirLogin = user.Sudir,
                FullName = user.Fio,
                GeneratedLiveLinksCount = gllc?.Count ?? 0,
                GeneratedArchiveLinksCount = galc?.Count ?? 0,
                LiveLinkClicks = llvc?.Count ?? 0,
                ArchiveLinkClicks = alvc?.Count ?? 0,
                AuthenticatedLinkClicks = alc?.Count ?? 0,
                UniqueCamerasCount = ucc?.Count ?? 0
            };
    }
}