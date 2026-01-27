using CommunityCar.Domain.Entities.Localization;

namespace CommunityCar.Application.Common.Interfaces.Services.Localization;

public interface ILocalizationService
{
    // Culture management
    Task<List<LocalizationCulture>> GetSupportedCulturesAsync();
    Task<LocalizationCulture?> GetCultureByNameAsync(string name);
    Task<Guid> AddCultureAsync(LocalizationCulture culture);
    Task UpdateCultureAsync(LocalizationCulture culture);
    Task DeleteCultureAsync(Guid id);

    // Resource management
    Task<string?> GetResourceValueAsync(string key, string culture, string? resourceGroup = null);
    Task<Dictionary<string, string>> GetResourcesByCultureAsync(string culture, string? resourceGroup = null);
    Task<List<LocalizationResource>> GetAllResourcesAsync(string? culture = null, string? resourceGroup = null);
    Task<(List<LocalizationResource> Items, int TotalCount)> GetPaginatedResourcesAsync(string? culture = null, string? resourceGroup = null, string? search = null, int page = 1, int pageSize = 20);
    Task<List<string>> GetResourceGroupsAsync();
    Task SetResourceValueAsync(string key, string value, string culture, string? resourceGroup = null);
    Task DeleteResourceAsync(Guid id);

    // Import/Export
    Task ImportResourcesAsync(Dictionary<string, string> resources, string culture, string? resourceGroup = null);
}



