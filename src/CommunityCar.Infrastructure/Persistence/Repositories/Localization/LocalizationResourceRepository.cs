using CommunityCar.Application.Common.Interfaces.Repositories.Localization;
using CommunityCar.Domain.Entities.Localization;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Localization;

public class LocalizationResourceRepository : BaseRepository<LocalizationResource>, ILocalizationResourceRepository
{
    public LocalizationResourceRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<LocalizationResource>> GetResourcesByCultureAsync(string culture)
    {
        return await Context.LocalizationResources
            .Where(r => r.Culture == culture)
            .OrderBy(r => r.Key)
            .ToListAsync();
    }

    public async Task<LocalizationResource?> GetResourceAsync(string key, string culture)
    {
        return await Context.LocalizationResources
            .FirstOrDefaultAsync(r => r.Key == key && r.Culture == culture);
    }

    public async Task<LocalizationResource> CreateResourceAsync(LocalizationResource resource)
    {
        await Context.LocalizationResources.AddAsync(resource);
        await Context.SaveChangesAsync();
        return resource;
    }

    public async Task<LocalizationResource> UpdateResourceAsync(LocalizationResource resource)
    {
        Context.LocalizationResources.Update(resource);
        await Context.SaveChangesAsync();
        return resource;
    }

    public async Task<bool> DeleteResourceAsync(Guid id)
    {
        var resource = await Context.LocalizationResources.FindAsync(id);
        if (resource == null)
            return false;

        Context.LocalizationResources.Remove(resource);
        await Context.SaveChangesAsync();
        return true;
    }

    public async Task<List<LocalizationResource>> GetAllResourcesAsync()
    {
        return await Context.LocalizationResources
            .OrderBy(r => r.Culture)
            .ThenBy(r => r.Key)
            .ToListAsync();
    }
}