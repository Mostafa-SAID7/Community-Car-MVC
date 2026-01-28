using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Domain.Entities.Community.Stories;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Community;

public class StoriesRepository : BaseRepository<Story>, IStoriesRepository
{
    public StoriesRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Story>> GetActiveAsync()
    {
        return await Context.Set<Story>()
            .Where(s => s.IsActive && !s.IsArchived && s.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Story>> GetExpiredAsync()
    {
        return await Context.Set<Story>()
            .Where(s => s.ExpiresAt <= DateTime.UtcNow)
            .OrderByDescending(s => s.ExpiresAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Story>> GetByAuthorAsync(Guid authorId)
    {
        return await Context.Set<Story>()
            .Where(s => s.AuthorId == authorId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Story>> GetFeaturedAsync()
    {
        return await Context.Set<Story>()
            .Where(s => s.IsFeatured && s.IsActive)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Story>> GetHighlightedAsync()
    {
        return await Context.Set<Story>()
            .Where(s => s.IsHighlighted && s.IsActive)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Story>> GetByVisibilityAsync(StoryVisibility visibility)
    {
        return await Context.Set<Story>()
            .Where(s => s.Visibility == visibility && s.IsActive)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Story>> GetByCarMakeAsync(string carMake)
    {
        return await Context.Set<Story>()
            .Where(s => s.CarMake != null && s.CarMake.ToLower() == carMake.ToLower() && s.IsActive)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Story>> GetByTagAsync(string tag)
    {
        return await Context.Set<Story>()
            .Where(s => s.Tags.Contains(tag.ToLower()) && s.IsActive)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Story>> GetByLocationAsync(double latitude, double longitude, double radiusKm)
    {
        // Note: This is a simplified implementation. In production, you'd use proper geospatial queries
        return await Context.Set<Story>()
            .Where(s => s.Latitude.HasValue && s.Longitude.HasValue && s.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetPopularTagsAsync(int count)
    {
        var allStories = await Context.Set<Story>()
            .Where(s => s.IsActive)
            .ToListAsync();

        return allStories
            .SelectMany(s => s.Tags)
            .GroupBy(t => t)
            .OrderByDescending(g => g.Count())
            .Take(count)
            .Select(g => g.Key)
            .ToList();
    }

    public async Task<IEnumerable<string>> GetAvailableCarMakesAsync()
    {
        return await Context.Set<Story>()
            .Where(s => s.CarMake != null && s.IsActive)
            .Select(s => s.CarMake!)
            .Distinct()
            .OrderBy(m => m)
            .ToListAsync();
    }

    public async Task<int> GetActiveCountByAuthorAsync(Guid authorId)
    {
        return await Context.Set<Story>()
            .CountAsync(s => s.AuthorId == authorId && s.IsActive && s.ExpiresAt > DateTime.UtcNow);
    }

    public async Task<bool> DeleteExpiredAsync()
    {
        var expiredStories = await Context.Set<Story>()
            .Where(s => s.ExpiresAt <= DateTime.UtcNow && !s.IsArchived)
            .ToListAsync();

        if (expiredStories.Any())
        {
            Context.Set<Story>().RemoveRange(expiredStories);
            return true;
        }

        return false;
    }
}

