using CommunityCar.Domain.Entities.Localization;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Localization;

public interface ILocalizationCultureRepository
{
    Task<List<LocalizationCulture>> GetSupportedCulturesAsync();
    Task<LocalizationCulture?> GetCultureByNameAsync(string name);
    Task<LocalizationCulture> CreateCultureAsync(LocalizationCulture culture);
    Task<LocalizationCulture> UpdateCultureAsync(LocalizationCulture culture);
    Task<bool> DeleteCultureAsync(Guid id);
}