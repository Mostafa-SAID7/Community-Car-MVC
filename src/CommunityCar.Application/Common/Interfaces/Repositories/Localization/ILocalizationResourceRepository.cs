using CommunityCar.Domain.Entities.Localization;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Localization;

public interface ILocalizationResourceRepository
{
    Task<List<LocalizationResource>> GetResourcesByCultureAsync(string culture);
    Task<LocalizationResource?> GetResourceAsync(string key, string culture);
    Task<LocalizationResource> CreateResourceAsync(LocalizationResource resource);
    Task<LocalizationResource> UpdateResourceAsync(LocalizationResource resource);
    Task<bool> DeleteResourceAsync(Guid id);
    Task<List<LocalizationResource>> GetAllResourcesAsync();
}