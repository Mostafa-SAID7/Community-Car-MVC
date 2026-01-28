using CommunityCar.Application.Common.Interfaces.Repositories.Profile;
using CommunityCar.Domain.Entities.Account.Media;
using CommunityCar.Domain.Enums.Account;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Profile;

public class UserGalleryRepository : BaseRepository<UserGallery>, IUserGalleryRepository
{
    public UserGalleryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<UserGallery>> GetUserGalleryAsync(Guid userId, bool publicOnly = false)
    {
        var query = Context.Set<UserGallery>()
            .Where(g => g.UserId == userId && !g.IsDeleted);

        if (publicOnly)
        {
            query = query.Where(g => g.IsPublic);
        }

        return await query
            .OrderByDescending(g => g.UploadedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserGallery>> GetFeaturedGalleryAsync(Guid userId)
    {
        return await Context.Set<UserGallery>()
            .Where(g => g.UserId == userId && g.IsFeatured && g.IsPublic && !g.IsDeleted)
            .OrderByDescending(g => g.UploadedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserGallery>> GetGalleryByTypeAsync(Guid userId, MediaType mediaType, bool publicOnly = false)
    {
        var query = Context.Set<UserGallery>()
            .Where(g => g.UserId == userId && g.MediaType == mediaType && !g.IsDeleted);

        if (publicOnly)
        {
            query = query.Where(g => g.IsPublic);
        }

        return await query
            .OrderByDescending(g => g.UploadedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserGallery>> GetGalleryByTagAsync(Guid userId, string tag, bool publicOnly = false)
    {
        var query = Context.Set<UserGallery>()
            .Where(g => g.UserId == userId && !g.IsDeleted);

        if (publicOnly)
        {
            query = query.Where(g => g.IsPublic);
        }

        // Filter by tag - assuming Tags is stored as JSON array
        query = query.Where(g => g.Tags != null && g.Tags.Contains($"\"{tag}\""));

        return await query
            .OrderByDescending(g => g.UploadedAt)
            .ToListAsync();
    }

    public async Task<int> GetGalleryCountAsync(Guid userId, bool publicOnly = false)
    {
        var query = Context.Set<UserGallery>()
            .Where(g => g.UserId == userId && !g.IsDeleted);

        if (publicOnly)
        {
            query = query.Where(g => g.IsPublic);
        }

        return await query.CountAsync();
    }

    public async Task<IEnumerable<UserGallery>> GetRecentGalleryAsync(Guid userId, int count = 10, bool publicOnly = false)
    {
        var query = Context.Set<UserGallery>()
            .Where(g => g.UserId == userId && !g.IsDeleted);

        if (publicOnly)
        {
            query = query.Where(g => g.IsPublic);
        }

        return await query
            .OrderByDescending(g => g.UploadedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserGallery>> GetPopularGalleryAsync(Guid userId, int count = 10, bool publicOnly = false)
    {
        var query = Context.Set<UserGallery>()
            .Where(g => g.UserId == userId && !g.IsDeleted);

        if (publicOnly)
        {
            query = query.Where(g => g.IsPublic);
        }

        return await query
            .OrderByDescending(g => g.ViewCount)
            .ThenByDescending(g => g.LikeCount)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetPopularTagsAsync(Guid userId)
    {
        var galleryItems = await Context.Set<UserGallery>()
            .Where(g => g.UserId == userId && g.IsPublic && !g.IsDeleted && g.Tags != null)
            .Select(g => g.Tags)
            .ToListAsync();

        var allTags = new List<string>();
        
        foreach (var tagJson in galleryItems)
        {
            if (!string.IsNullOrEmpty(tagJson))
            {
                try
                {
                    var tags = JsonSerializer.Deserialize<string[]>(tagJson);
                    if (tags != null)
                    {
                        allTags.AddRange(tags);
                    }
                }
                catch (JsonException)
                {
                    // Skip invalid JSON
                    continue;
                }
            }
        }

        return allTags
            .GroupBy(t => t.ToLowerInvariant())
            .OrderByDescending(g => g.Count())
            .Take(20)
            .Select(g => g.Key)
            .ToList();
    }

    public async Task<UserGallery?> GetGalleryItemAsync(Guid itemId, Guid? userId = null)
    {
        var query = Context.Set<UserGallery>()
            .Where(g => g.Id == itemId && !g.IsDeleted);

        // If userId is provided, check if it's the owner or if the item is public
        if (userId.HasValue)
        {
            query = query.Where(g => g.UserId == userId.Value || g.IsPublic);
        }
        else
        {
            // If no userId provided, only return public items
            query = query.Where(g => g.IsPublic);
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task<bool> CanAccessGalleryItemAsync(Guid itemId, Guid? userId = null)
    {
        var item = await Context.Set<UserGallery>()
            .Where(g => g.Id == itemId && !g.IsDeleted)
            .FirstOrDefaultAsync();

        if (item == null)
            return false;

        // Owner can always access
        if (userId.HasValue && item.UserId == userId.Value)
            return true;

        // Public items can be accessed by anyone
        return item.IsPublic;
    }
}
