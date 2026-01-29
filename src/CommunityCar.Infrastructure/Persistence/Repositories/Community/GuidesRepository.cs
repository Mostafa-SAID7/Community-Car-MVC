using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Application.Features.Guides.ViewModels;
using CommunityCar.Domain.Entities.Community.Guides;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Community;

public class GuidesRepository : BaseRepository<Guide>, IGuidesRepository
{
    public GuidesRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Guide?> GetGuideByIdAsync(Guid id)
    {
        return await Context.Guides
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<Guide?> GetGuideWithAuthorAsync(Guid id)
    {
        return await Context.Guides
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<IEnumerable<Guide>> GetGuidesAsync(GuideFilterVM filter)
    {
        var query = Context.Guides.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            query = query.Where(g => 
                g.Title.Contains(filter.Search) ||
                (g.Summary != null && g.Summary.Contains(filter.Search)) ||
                g.Content.Contains(filter.Search));
        }

        if (!string.IsNullOrWhiteSpace(filter.Category) && filter.Category != "all")
        {
            query = query.Where(g => g.Category == filter.Category);
        }

        if (filter.Difficulty.HasValue)
        {
            query = query.Where(g => g.Difficulty == filter.Difficulty.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Tag))
        {
            query = query.Where(g => g.Tags.Contains(filter.Tag));
        }

        if (filter.IsVerified.HasValue)
        {
            query = query.Where(g => g.IsVerified == filter.IsVerified.Value);
        }

        if (filter.IsFeatured.HasValue)
        {
            query = query.Where(g => g.IsFeatured == filter.IsFeatured.Value);
        }

        if (filter.IsPublished.HasValue)
        {
            query = query.Where(g => g.IsPublished == filter.IsPublished.Value);
        }
        else
        {
            // Default to published guides only
            query = query.Where(g => g.IsPublished);
        }

        if (filter.AuthorId.HasValue)
        {
            query = query.Where(g => g.AuthorId == filter.AuthorId.Value);
        }

        // Apply sorting
        query = filter.SortBy?.ToLower() switch
        {
            "popular" => query.OrderByDescending(g => g.ViewCount),
            "rating" => query.OrderByDescending(g => g.AverageRating).ThenByDescending(g => g.RatingCount),
            "bookmarks" => query.OrderByDescending(g => g.BookmarkCount),
            "oldest" => query.OrderBy(g => g.PublishedAt ?? g.CreatedAt),
            "newest" or _ => query.OrderByDescending(g => g.PublishedAt ?? g.CreatedAt),
        };

        // Apply pagination
        return await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Guide>> GetFeaturedGuidesAsync(int count = 10)
    {
        return await Context.Guides
            .Where(g => g.IsFeatured && g.IsPublished)
            .OrderByDescending(g => g.PublishedAt ?? g.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Guide>> GetVerifiedGuidesAsync(int count = 10)
    {
        return await Context.Guides
            .Where(g => g.IsVerified && g.IsPublished)
            .OrderByDescending(g => g.AverageRating)
            .ThenByDescending(g => g.ViewCount)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Guide>> GetPopularGuidesAsync(int count = 10)
    {
        return await Context.Guides
            .Where(g => g.IsPublished)
            .OrderByDescending(g => g.ViewCount)
            .ThenByDescending(g => g.AverageRating)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Guide>> GetRecentGuidesAsync(int count = 10)
    {
        return await Context.Guides
            .Where(g => g.IsPublished)
            .OrderByDescending(g => g.PublishedAt ?? g.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Guide>> GetGuidesByAuthorAsync(Guid authorId, int count = 10)
    {
        return await Context.Guides
            .Where(g => g.AuthorId == authorId && g.IsPublished)
            .OrderByDescending(g => g.PublishedAt ?? g.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Guide>> GetRelatedGuidesAsync(Guid guideId, int count = 5)
    {
        var guide = await GetGuideByIdAsync(guideId);
        if (guide == null) return Enumerable.Empty<Guide>();

        var query = Context.Guides
            .Where(g => g.Id != guideId && g.IsPublished);

        // Find guides with same category or tags
        if (!string.IsNullOrWhiteSpace(guide.Category))
        {
            query = query.Where(g => g.Category == guide.Category);
        }

        return await query
            .OrderByDescending(g => g.AverageRating)
            .ThenByDescending(g => g.ViewCount)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Guide>> SearchGuidesAsync(string searchTerm, int count = 20)
    {
        return await Context.Guides
            .Where(g => g.IsPublished && (
                g.Title.Contains(searchTerm) ||
                (g.Summary != null && g.Summary.Contains(searchTerm)) ||
                g.Content.Contains(searchTerm) ||
                g.Tags.Any(t => t.Contains(searchTerm))))
            .OrderByDescending(g => g.AverageRating)
            .ThenByDescending(g => g.ViewCount)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Guide> CreateGuideAsync(Guide guide)
    {
        Context.Guides.Add(guide);
        await Context.SaveChangesAsync();
        return guide;
    }

    public async Task<Guide> UpdateGuideAsync(Guide guide)
    {
        Context.Guides.Update(guide);
        await Context.SaveChangesAsync();
        return guide;
    }

    public async Task DeleteGuideAsync(Guid id)
    {
        var guide = await GetGuideByIdAsync(id);
        if (guide != null)
        {
            Context.Guides.Remove(guide);
            await Context.SaveChangesAsync();
        }
    }

    public async Task<int> GetTotalCountAsync(GuideFilterVM filter)
    {
        var query = Context.Guides.AsQueryable();

        // Apply same filters as GetGuidesAsync
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            query = query.Where(g => 
                g.Title.Contains(filter.Search) ||
                (g.Summary != null && g.Summary.Contains(filter.Search)) ||
                g.Content.Contains(filter.Search));
        }

        if (!string.IsNullOrWhiteSpace(filter.Category) && filter.Category != "all")
        {
            query = query.Where(g => g.Category == filter.Category);
        }

        if (filter.Difficulty.HasValue)
        {
            query = query.Where(g => g.Difficulty == filter.Difficulty.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Tag))
        {
            query = query.Where(g => g.Tags.Contains(filter.Tag));
        }

        if (filter.IsVerified.HasValue)
        {
            query = query.Where(g => g.IsVerified == filter.IsVerified.Value);
        }

        if (filter.IsFeatured.HasValue)
        {
            query = query.Where(g => g.IsFeatured == filter.IsFeatured.Value);
        }

        if (filter.IsPublished.HasValue)
        {
            query = query.Where(g => g.IsPublished == filter.IsPublished.Value);
        }
        else
        {
            query = query.Where(g => g.IsPublished);
        }

        if (filter.AuthorId.HasValue)
        {
            query = query.Where(g => g.AuthorId == filter.AuthorId.Value);
        }

        return await query.CountAsync();
    }

    public async Task<IEnumerable<string>> GetCategoriesAsync()
    {
        return await Context.Guides
            .Where(g => g.IsPublished && !string.IsNullOrWhiteSpace(g.Category))
            .Select(g => g.Category!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetPopularTagsAsync(int count = 20)
    {
        var allTags = await Context.Guides
            .Where(g => g.IsPublished)
            .SelectMany(g => g.Tags)
            .ToListAsync();

        return allTags
            .GroupBy(t => t)
            .OrderByDescending(g => g.Count())
            .Take(count)
            .Select(g => g.Key)
            .ToList();
    }

    public async Task<bool> IsBookmarkedByUserAsync(Guid guideId, Guid userId)
    {
        return await Context.Bookmarks
            .AnyAsync(b => b.EntityId == guideId && b.UserId == userId && b.EntityType == EntityType.Guide);
    }

    public async Task<double?> GetUserRatingAsync(Guid guideId, Guid userId)
    {
        var rating = await Context.Ratings
            .FirstOrDefaultAsync(r => r.EntityId == guideId && r.UserId == userId && r.EntityType == EntityType.Guide);
        
        return rating?.Value;
    }

    public async Task IncrementViewCountAsync(Guid guideId)
    {
        var guide = await GetGuideByIdAsync(guideId);
        if (guide != null)
        {
            guide.IncrementViewCount();
            await Context.SaveChangesAsync();
        }
    }

    public async Task<int> GetCountByUserAndDateAsync(Guid userId, DateTime date)
    {
        return await Context.Guides
            .CountAsync(g => g.AuthorId == userId && g.CreatedAt.Date == date.Date);
    }
}
