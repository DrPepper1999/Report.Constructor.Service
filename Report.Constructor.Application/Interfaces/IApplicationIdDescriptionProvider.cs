namespace Report.Constructor.Application.Interfaces;

public interface IApplicationIdDescriptionProvider
{
    Task<Dictionary<Guid, string>> GetDescriptionsAsync();
}