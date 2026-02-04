using CommunityCar.Application.Common.Interfaces.Repositories.Localization;
using CommunityCar.Domain.Entities.Localization;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Localization;

public class LocalizationCultureRepository : BaseRepository<LocalizationCulture>, ILocalizationCultureRepository
{
    public LocalizationCultureRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<LocalizationCulture>> GetSupportedCulturesAsync()
    {
        return await Context.LocalizationCultures
            .Where(c => c.IsEnabled)
            .OrderBy(c => c.DisplayName)
            .ToListAsync();
    }

    public async Task<LocalizationCulture?> GetCultureByNameAsync(string name)
    {
        return await Context.LocalizationCultures
            .FirstOrDefaultAsync(c => c.Name == name);
    }

    public async Task<LocalizationCulture> CreateCultureAsync(LocalizationCulture culture)
    {
        await Context.LocalizationCultures.AddAsync(culture);
        await Context.SaveChangesAsync();
        return culture;
    }

    public async Task<LocalizationCulture> UpdateCultureAsync(LocalizationCulture culture)
    {
        Context.LocalizationCultures.Update(culture);
        await Context.SaveChangesAsync();
        return culture;
    }

    public async Task<bool> DeleteCultureAsync(Guid id)
    {
        var culture = await Context.LocalizationCultures.FindAsync(id);
        if (culture == null)
            return false;

        Context.LocalizationCultures.Remove(culture);
        await Context.SaveChangesAsync();
        return true;
    }
}