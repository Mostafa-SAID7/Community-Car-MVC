using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Localization;
using CommunityCar.Domain.Entities.Localization;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Application.Services.Dashboard.Localization;

public class LocalizationService : ILocalizationService
{
    private readonly IUnitOfWork _unitOfWork;

    public LocalizationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<LocalizationCulture>> GetSupportedCulturesAsync()
    {
        var cultures = await _unitOfWork.Context.Set<LocalizationCulture>()
            .Where(c => c.IsEnabled)
            .OrderBy(c => c.DisplayName)
            .ToListAsync();

        return cultures;
    }

    public async Task<LocalizationCulture?> GetCultureByNameAsync(string name)
    {
        return await _unitOfWork.Context.Set<LocalizationCulture>()
            .FirstOrDefaultAsync(c => c.Name == name);
    }

    public async Task<Guid> AddCultureAsync(LocalizationCulture culture)
    {
        culture.Id = Guid.NewGuid();
        culture.CreatedAt = DateTime.UtcNow;
        
        await _unitOfWork.Context.Set<LocalizationCulture>().AddAsync(culture);
        await _unitOfWork.SaveChangesAsync();
        
        return culture.Id;
    }

    public async Task UpdateCultureAsync(LocalizationCulture culture)
    {
        culture.UpdatedAt = DateTime.UtcNow;
        
        _unitOfWork.Context.Set<LocalizationCulture>().Update(culture);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteCultureAsync(Guid id)
    {
        var culture = await _unitOfWork.Context.Set<LocalizationCulture>()
            .FindAsync(id);
            
        if (culture != null)
        {
            _unitOfWork.Context.Set<LocalizationCulture>().Remove(culture);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<string?> GetResourceValueAsync(string key, string culture, string? resourceGroup = null)
    {
        var resource = await _unitOfWork.Context.Set<LocalizationResource>()
            .FirstOrDefaultAsync(r => r.Key == key && 
                                    r.Culture == culture && 
                                    (resourceGroup == null || r.ResourceGroup == resourceGroup));

        return resource?.Value;
    }

    public async Task<Dictionary<string, string>> GetResourcesByCultureAsync(string culture, string? resourceGroup = null)
    {
        var resources = await _unitOfWork.Context.Set<LocalizationResource>()
            .Where(r => r.Culture == culture && 
                       (resourceGroup == null || r.ResourceGroup == resourceGroup))
            .ToDictionaryAsync(r => r.Key, r => r.Value);

        return resources;
    }

    public async Task<List<LocalizationResource>> GetAllResourcesAsync(string? culture = null, string? resourceGroup = null)
    {
        var query = _unitOfWork.Context.Set<LocalizationResource>().AsQueryable();

        if (!string.IsNullOrEmpty(culture))
            query = query.Where(r => r.Culture == culture);

        if (!string.IsNullOrEmpty(resourceGroup))
            query = query.Where(r => r.ResourceGroup == resourceGroup);

        return await query.OrderBy(r => r.Key).ToListAsync();
    }

    public async Task<(List<LocalizationResource> Items, int TotalCount)> GetPaginatedResourcesAsync(
        string? culture = null, 
        string? resourceGroup = null, 
        string? search = null, 
        int page = 1, 
        int pageSize = 20)
    {
        var query = _unitOfWork.Context.Set<LocalizationResource>().AsQueryable();

        if (!string.IsNullOrEmpty(culture))
            query = query.Where(r => r.Culture == culture);

        if (!string.IsNullOrEmpty(resourceGroup))
            query = query.Where(r => r.ResourceGroup == resourceGroup);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(r => r.Key.Contains(search) || r.Value.Contains(search));

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(r => r.Key)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<List<string>> GetResourceGroupsAsync()
    {
        var groups = await _unitOfWork.Context.Set<LocalizationResource>()
            .Where(r => !string.IsNullOrEmpty(r.ResourceGroup))
            .Select(r => r.ResourceGroup!)
            .Distinct()
            .OrderBy(g => g)
            .ToListAsync();

        return groups;
    }

    public async Task SetResourceValueAsync(string key, string value, string culture, string? resourceGroup = null)
    {
        var resource = await _unitOfWork.Context.Set<LocalizationResource>()
            .FirstOrDefaultAsync(r => r.Key == key && 
                                    r.Culture == culture && 
                                    (resourceGroup == null || r.ResourceGroup == resourceGroup));

        if (resource != null)
        {
            resource.Value = value;
            resource.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Context.Set<LocalizationResource>().Update(resource);
        }
        else
        {
            resource = new LocalizationResource
            {
                Id = Guid.NewGuid(),
                Key = key,
                Value = value,
                Culture = culture,
                ResourceGroup = resourceGroup,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Context.Set<LocalizationResource>().AddAsync(resource);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteResourceAsync(Guid id)
    {
        var resource = await _unitOfWork.Context.Set<LocalizationResource>()
            .FindAsync(id);
            
        if (resource != null)
        {
            _unitOfWork.Context.Set<LocalizationResource>().Remove(resource);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task ImportResourcesAsync(Dictionary<string, string> resources, string culture, string? resourceGroup = null)
    {
        foreach (var kvp in resources)
        {
            await SetResourceValueAsync(kvp.Key, kvp.Value, culture, resourceGroup);
        }
    }
}