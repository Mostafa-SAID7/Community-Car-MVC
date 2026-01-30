using CommunityCar.Application.Common.Interfaces.Repositories.Account;
using CommunityCar.Domain.Entities.Account.Media;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Account.Media;

/// <summary>
/// Repository implementation for UserGallery entity operations
/// </summary>
public class UserGalleryRepository : BaseRepository<UserGallery>, IUserGalleryRepository
{
    public UserGalleryRepository(ApplicationDbContext context) : base(context)
    {
    }

    #region Gallery Management

    public async Task<IEnumerable<UserGallery>> GetUserGalleryAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        return await Context.UserGalleries
            .Where(ug => ug.UserId == userId)
            .OrderBy(ug => ug.DisplayOrder)
            .ThenByDescending(ug => ug.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<UserGallery?> GetGalleryItemAsync(Guid userId, Guid itemId)
    {
        return await Context.UserGalleries
            .FirstOrDefaultAsync(ug => ug.UserId == userId && ug.Id == itemId);
    }

    public async Task<bool> AddGalleryItemAsync(Guid userId, string imageUrl, string? caption = null, Dictionary<string, object>? metadata = null)
    {
        var galleryItem = UserGallery.Create(userId, imageUrl, caption, metadata);
        await AddAsync(galleryItem);
        return true;
    }

    public async Task<bool> UpdateGalleryItemAsync(Guid itemId, string? caption = null, Dictionary<string, object>? metadata = null)
    {
        var galleryItem = await GetByIdAsync(itemId);
        
        if (galleryItem == null) return false;

        galleryItem.UpdateCaption(caption);
        if (metadata != null)
        {
            galleryItem.UpdateMetadata(metadata);
        }
        
        await UpdateAsync(galleryItem);
        return true;
    }

    public async Task<bool> RemoveGalleryItemAsync(Guid itemId)
    {
        var galleryItem = await GetByIdAsync(itemId);
        
        if (galleryItem == null) return false;

        await DeleteAsync(galleryItem);
        return true;
    }

    public async Task<bool> RemoveUserGalleryAsync(Guid userId)
    {
        var galleryItems = await Context.UserGalleries
            .Where(ug => ug.UserId == userId)
            .ToListAsync();

        if (galleryItems.Any())
        {
            await DeleteRangeAsync(galleryItems);
            return true;
        }

        return false;
    }

    #endregion

    #region Gallery Organization

    public async Task<bool> SetGalleryItemOrderAsync(Guid userId, Dictionary<Guid, int> itemOrders)
    {
        var galleryItems = await Context.UserGalleries
            .Where(ug => ug.UserId == userId && itemOrders.Keys.Contains(ug.Id))
            .ToListAsync();

        foreach (var item in galleryItems)
        {
            if (itemOrders.TryGetValue(item.Id, out var order))
            {
                item.SetDisplayOrder(order);
            }
        }

        if (galleryItems.Any())
        {
            Context.UserGalleries.UpdateRange(galleryItems);
            return true;
        }

        return false;
    }

    public async Task<IEnumerable<UserGallery>> GetGalleryByTagAsync(Guid userId, string tag)
    {
        return await Context.UserGalleries
            .Where(ug => ug.UserId == userId && ug.Tags.Contains(tag))
            .OrderBy(ug => ug.DisplayOrder)
            .ThenByDescending(ug => ug.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> AddTagToGalleryItemAsync(Guid itemId, string tag)
    {
        var galleryItem = await GetByIdAsync(itemId);
        
        if (galleryItem == null) return false;

        galleryItem.AddTag(tag);
        await UpdateAsync(galleryItem);
        return true;
    }

    public async Task<bool> RemoveTagFromGalleryItemAsync(Guid itemId, string tag)
    {
        var galleryItem = await GetByIdAsync(itemId);
        
        if (galleryItem == null) return false;

        galleryItem.RemoveTag(tag);
        await UpdateAsync(galleryItem);
        return true;
    }

    public async Task<IEnumerable<string>> GetGalleryTagsAsync(Guid userId)
    {
        var galleryItems = await Context.UserGalleries
            .Where(ug => ug.UserId == userId)
            .Select(ug => ug.Tags)
            .ToListAsync();

        return galleryItems
            .Where(tags => !string.IsNullOrEmpty(tags))
            .SelectMany(tags => System.Text.Json.JsonSerializer.Deserialize<List<string>>(tags) ?? new List<string>())
            .Distinct()
            .OrderBy(tag => tag)
            .ToList();
    }

    #endregion

    #region Gallery Analytics

    public async Task<int> GetGalleryItemCountAsync(Guid userId)
    {
        return await Context.UserGalleries
            .Where(ug => ug.UserId == userId)
            .CountAsync();
    }

    public async Task<IEnumerable<UserGallery>> GetRecentGalleryItemsAsync(Guid userId, int count = 10)
    {
        return await Context.UserGalleries
            .Where(ug => ug.UserId == userId)
            .OrderByDescending(ug => ug.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserGallery>> GetPopularGalleryItemsAsync(Guid userId, int count = 10)
    {
        return await Context.UserGalleries
            .Where(ug => ug.UserId == userId)
            .OrderByDescending(ug => ug.ViewCount)
            .Take(count)
            .ToListAsync();
    }

    public async Task<long> GetGalleryStorageSizeAsync(Guid userId)
    {
        return await Context.UserGalleries
            .Where(ug => ug.UserId == userId)
            .SumAsync(ug => ug.FileSize);
    }

    #endregion

    #region Gallery Privacy

    public async Task<bool> SetGalleryItemVisibilityAsync(Guid itemId, bool isPublic)
    {
        var galleryItem = await GetByIdAsync(itemId);
        
        if (galleryItem == null) return false;

        galleryItem.SetVisibility(isPublic);
        await UpdateAsync(galleryItem);
        return true;
    }

    public async Task<IEnumerable<UserGallery>> GetPublicGalleryAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        return await Context.UserGalleries
            .Where(ug => ug.UserId == userId && ug.IsPublic)
            .OrderBy(ug => ug.DisplayOrder)
            .ThenByDescending(ug => ug.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserGallery>> GetPrivateGalleryAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        return await Context.UserGalleries
            .Where(ug => ug.UserId == userId && !ug.IsPublic)
            .OrderBy(ug => ug.DisplayOrder)
            .ThenByDescending(ug => ug.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    #endregion
}