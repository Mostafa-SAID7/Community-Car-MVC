using CommunityCar.Application.Common.Interfaces.Data;
using CommunityCar.Application.Common.Interfaces.Services.Localization;
using CommunityCar.Domain.Entities.Localization;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Application.Services.Localization;

public class LocalizationService : ILocalizationService
{
    private readonly IApplicationDbContext _context;

    public LocalizationService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<LocalizationCulture>> GetSupportedCulturesAsync()
    {
        return await _context.LocalizationCultures
            .Where(c => c.IsActive)
            .ToListAsync();
    }

    public async Task<LocalizationCulture?> GetCultureByNameAsync(string name)
    {
        return await _context.LocalizationCultures
            .FirstOrDefaultAsync(c => c.Name == name);
    }

    public async Task<Guid> AddCultureAsync(LocalizationCulture culture)
    {
        if (culture.IsDefault)
        {
            var currentDefault = await _context.LocalizationCultures.FirstOrDefaultAsync(c => c.IsDefault);
            if (currentDefault != null) currentDefault.IsDefault = false;
        }

        _context.LocalizationCultures.Add(culture);
        await _context.SaveChangesAsync();
        return culture.Id;
    }

    public async Task UpdateCultureAsync(LocalizationCulture culture)
    {
        if (culture.IsDefault)
        {
            var currentDefault = await _context.LocalizationCultures
                .FirstOrDefaultAsync(c => c.IsDefault && c.Id != culture.Id);
            if (currentDefault != null) currentDefault.IsDefault = false;
        }

        _context.LocalizationCultures.Update(culture);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCultureAsync(Guid id)
    {
        var culture = await _context.LocalizationCultures.FindAsync(id);
        if (culture != null)
        {
            _context.LocalizationCultures.Remove(culture);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<string?> GetResourceValueAsync(string key, string culture, string? resourceGroup = null)
    {
        var query = _context.LocalizationResources
            .Where(r => r.Key == key && r.Culture == culture);

        if (!string.IsNullOrEmpty(resourceGroup))
        {
            query = query.Where(r => r.ResourceGroup == resourceGroup);
        }

        var resource = await query.FirstOrDefaultAsync();
        return resource?.Value;
    }

    public async Task<Dictionary<string, string>> GetResourcesByCultureAsync(string culture, string? resourceGroup = null)
    {
        var query = _context.LocalizationResources
            .Where(r => r.Culture == culture);

        if (!string.IsNullOrEmpty(resourceGroup))
        {
            query = query.Where(r => r.ResourceGroup == resourceGroup);
        }

        return await query.ToDictionaryAsync(r => r.Key, r => r.Value);
    }

    public async Task<List<LocalizationResource>> GetAllResourcesAsync(string? culture = null, string? resourceGroup = null)
    {
        var query = _context.LocalizationResources.AsQueryable();

        if (!string.IsNullOrEmpty(culture))
        {
            query = query.Where(r => r.Culture == culture);
        }

        if (!string.IsNullOrEmpty(resourceGroup))
        {
            query = query.Where(r => r.ResourceGroup == resourceGroup);
        }

        return await query.ToListAsync();
    }

    public async Task<(List<LocalizationResource> Items, int TotalCount)> GetPaginatedResourcesAsync(string? culture = null, string? resourceGroup = null, string? search = null, int page = 1, int pageSize = 20)
    {
        var query = _context.LocalizationResources.AsQueryable();

        if (!string.IsNullOrEmpty(culture))
        {
            query = query.Where(r => r.Culture == culture);
        }

        if (!string.IsNullOrEmpty(resourceGroup))
        {
            query = query.Where(r => r.ResourceGroup == resourceGroup);
        }

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(r => r.Key.Contains(search) || r.Value.Contains(search));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(r => r.ResourceGroup)
            .ThenBy(r => r.Key)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<List<string>> GetResourceGroupsAsync()
    {
        return await _context.LocalizationResources
            .Select(r => r.ResourceGroup ?? "Global")
            .Distinct()
            .OrderBy(g => g)
            .ToListAsync();
    }

    public async Task SetResourceValueAsync(string key, string value, string culture, string? resourceGroup = null)
    {
        var resource = await _context.LocalizationResources
            .FirstOrDefaultAsync(r => r.Key == key && r.Culture == culture && r.ResourceGroup == resourceGroup);

        if (resource != null)
        {
            resource.Value = value;
            _context.LocalizationResources.Update(resource);
        }
        else
        {
            _context.LocalizationResources.Add(new LocalizationResource
            {
                Key = key,
                Value = value,
                Culture = culture,
                ResourceGroup = resourceGroup
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteResourceAsync(Guid id)
    {
        var resource = await _context.LocalizationResources.FindAsync(id);
        if (resource != null)
        {
            _context.LocalizationResources.Remove(resource);
            await _context.SaveChangesAsync();
        }
    }

    public async Task ImportResourcesAsync(Dictionary<string, string> resources, string culture, string? resourceGroup = null)
    {
        foreach (var item in resources)
        {
            var existing = await _context.LocalizationResources
                .FirstOrDefaultAsync(r => r.Key == item.Key && r.Culture == culture && r.ResourceGroup == resourceGroup);

            if (existing != null)
            {
                existing.Value = item.Value;
            }
            else
            {
                _context.LocalizationResources.Add(new LocalizationResource
                {
                    Key = item.Key,
                    Value = item.Value,
                    Culture = culture,
                    ResourceGroup = resourceGroup
                });
            }
        }

        await _context.SaveChangesAsync();
    }
}
