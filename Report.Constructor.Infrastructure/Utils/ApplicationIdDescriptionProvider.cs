using Report.Constructor.Application.Interfaces;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.DAL.MoscowVideoDb.Entities;

namespace Report.Constructor.Infrastructure.Utils;

internal sealed class ApplicationIdDescriptionProvider : IApplicationIdDescriptionProvider
{
    private readonly IExternalServiceRepository _externalServiceRepository;
    
    public ApplicationIdDescriptionProvider(IExternalServiceRepository externalServiceRepository)
    {
        _externalServiceRepository = externalServiceRepository;
    }

    public async Task<Dictionary<Guid, string>> GetDescriptionsAsync()
    {
        var externalServices = await _externalServiceRepository.GetAllAsync();
        var externalServicesDict = externalServices.Concat(new[]
            {
                new ExternalService
                {
                    Id = Guid.Empty,
                    Key = Guid.Empty.ToString(),
                    Name = "Неизвестное приложение"
                }
            })
            .DistinctBy(x => x.Key)
            .ToDictionary(x => x.GuidKey, x => x.Name);

        return externalServicesDict;
    }
}