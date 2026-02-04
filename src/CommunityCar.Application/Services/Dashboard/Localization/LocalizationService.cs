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
        return await _unitOfWork.LocalizationCultures.GetSupportedCulturesAsync();
    }

    public async Task<LocalizationCulture?> GetCultureByNameAsync(string name)
    {
        return await _unitOfWork.LocalizationCultures.GetCultureByNameAsync(name);
    }

    public async Task<Guid> AddCultureAsync(LocalizationCulture culture)
    {
        await _unitOfWork.LocalizationCultures.CreateCultureAsync(culture);
        await _unitOfWork.SaveChangesAsync();
        
        return culture.Id;
    }

    public async Task UpdateCultureAsync(LocalizationCulture culture)
    {
        await _unitOfWork.LocalizationCultures.UpdateCultureAsync(culture);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteCultureAsync(Guid id)
    {
        await _unitOfWork.LocalizationCultures.DeleteCultureAsync(id);
            
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<string?> GetResourceValueAsync(string key, string culture, string? resourceGroup = null)
    {
        var resource = await _unitOfWork.LocalizationResources.GetResourceAsync(key, culture);
        return resource?.Value;
    }

    public async Task<Dictionary<string, string>> GetResourcesByCultureAsync(string culture, string? resourceGroup = null)
    {
        var resources = await _unitOfWork.LocalizationResources.GetResourcesByCultureAsync(culture);
        return resources.ToDictionary(r => r.Key, r => r.Value);
    }

    public async Task<List<LocalizationResource>> GetAllResourcesAsync(string? culture = null, string? resourceGroup = null)
    {
        var allResources = await _unitOfWork.LocalizationResources.GetAllResourcesAsync();
        var query = allResources.AsQueryable();

        if (!string.IsNullOrEmpty(culture))
            query = query.Where(r => r.Culture == culture);

        if (!string.IsNullOrEmpty(resourceGroup))
            query = query.Where(r => r.ResourceGroup == resourceGroup);

        return query.OrderBy(r => r.Key).ToList();
    }

    public async Task<(List<LocalizationResource> Items, int TotalCount)> GetPaginatedResourcesAsync(
        string? culture = null, 
        string? resourceGroup = null, 
        string? search = null, 
        int page = 1, 
        int pageSize = 20)
    {
        var allResources = await _unitOfWork.LocalizationResources.GetAllResourcesAsync();
        var query = allResources.AsQueryable();

        if (!string.IsNullOrEmpty(culture))
            query = query.Where(r => r.Culture == culture);

        if (!string.IsNullOrEmpty(resourceGroup))
            query = query.Where(r => r.ResourceGroup == resourceGroup);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(r => r.Key.Contains(search) || r.Value.Contains(search));

        var totalCount = query.Count();
        var items = query
            .OrderBy(r => r.Key)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return (items, totalCount);
    }

    public async Task<List<string>> GetResourceGroupsAsync()
    {
        var allResources = await _unitOfWork.LocalizationResources.GetAllResourcesAsync();
        var groups = allResources
            .Where(r => !string.IsNullOrEmpty(r.ResourceGroup))
            .Select(r => r.ResourceGroup!)
            .Distinct()
            .OrderBy(g => g)
            .ToList();

        return groups;
    }

    public async Task SetResourceValueAsync(string key, string value, string culture, string? resourceGroup = null)
    {
        var resource = await _unitOfWork.LocalizationResources.GetResourceAsync(key, culture);

        if (resource != null)
        {
            resource.Value = value;
            await _unitOfWork.LocalizationResources.UpdateResourceAsync(resource);
        }
        else
        {
            resource = new LocalizationResource
            {
                Key = key,
                Value = value,
                Culture = culture,
                ResourceGroup = resourceGroup
            };
            await _unitOfWork.LocalizationResources.CreateResourceAsync(resource);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteResourceAsync(Guid id)
    {
        await _unitOfWork.LocalizationResources.DeleteResourceAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ImportResourcesAsync(Dictionary<string, string> resources, string culture, string? resourceGroup = null)
    {
        foreach (var kvp in resources)
        {
            await SetResourceValueAsync(kvp.Key, kvp.Value, culture, resourceGroup);
        }
    }
}