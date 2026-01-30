using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Application.Features.Guides.ViewModels;
using CommunityCar.Domain.Entities.Community.Guides;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Community;

public class GuidesRepository : BaseRepository<Guide>, IGuidesRepository
{
    public GuidesRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<int> GetCountByUserAndDateAsync(Guid userId, DateTime date)
    {
        return await Context.Guides
            .CountAsync(g => g.CreatedBy == userId.ToString() && g.CreatedAt.Date == date.Date);
    }

    public async Task<Guide?> GetGuideWithAuthorAsync(Guid id)
    {
        return await Context.Guides
            .Include(g => g.Author)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<IEnumerable<Guide>> GetRelatedGuidesAsync(Guid guideId, int count)
    {
        var guide = await GetByIdAsync(guideId);
        if (guide == null) return Enumerable.Empty<Guide>();

        return await Context.Guides
            .Where(g => g.Id != guideId && g.Category == guide.Category)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Guide>> GetGuidesByAuthorAsync(Guid authorId, int count)
    {
        return await Context.Guides
            .Where(g => g.AuthorId == authorId)
            .OrderByDescending(g => g.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Guide>> GetGuidesAsync(GuideFilterVM filter)
    {
        var query = Context.Guides.AsQueryable();
        // Simple filtering for stub
        if (!string.IsNullOrEmpty(filter.Search))
            query = query.Where(g => g.Title.Contains(filter.Search));

        return await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync(GuideFilterVM filter)
    {
        var query = Context.Guides.AsQueryable();
        if (!string.IsNullOrEmpty(filter.Search))
            query = query.Where(g => g.Title.Contains(filter.Search));

        return await query.CountAsync();
    }

    public async Task<IEnumerable<string>> GetCategoriesAsync()
    {
        return await Context.Guides
            .Select(g => g.Category)
            .Distinct()
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetPopularTagsAsync(int count)
    {
        return await Task.FromResult(new List<string>());
    }

    public async Task<IEnumerable<Guide>> GetFeaturedGuidesAsync(int count)
    {
        return await Context.Guides
            .Where(g => g.IsFeatured)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Guide>> GetVerifiedGuidesAsync(int count)
    {
        return await Context.Guides
            .Where(g => g.IsVerified)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Guide>> GetPopularGuidesAsync(int count)
    {
        return await Context.Guides
            .OrderByDescending(g => g.ViewCount)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Guide>> GetRecentGuidesAsync(int count)
    {
        return await Context.Guides
            .OrderByDescending(g => g.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Guide>> SearchGuidesAsync(string searchTerm, int count)
    {
        return await Context.Guides
            .Where(g => g.Title.Contains(searchTerm))
            .Take(count)
            .ToListAsync();
    }

    public async Task<Guide> CreateGuideAsync(Guide guide)
    {
        await AddAsync(guide);
        return guide;
    }

    public async Task<Guide?> GetGuideByIdAsync(Guid id)
    {
        return await GetByIdAsync(id);
    }

    public async Task UpdateGuideAsync(Guide guide)
    {
        await UpdateAsync(guide);
    }

    public async Task DeleteGuideAsync(Guid id)
    {
        var guide = await GetByIdAsync(id);
        if (guide != null)
        {
            await DeleteAsync(guide);
        }
    }

    public async Task IncrementViewCountAsync(Guid guideId)
    {
        var guide = await GetByIdAsync(guideId);
        if (guide != null)
        {
            guide.IncrementViewCount();
            await UpdateAsync(guide);
        }
    }

    public async Task<bool> IsBookmarkedByUserAsync(Guid guideId, Guid userId)
    {
        return await Context.Bookmarks
            .AnyAsync(b => b.EntityId == guideId && b.UserId == userId);
    }

    public async Task<double?> GetUserRatingAsync(Guid guideId, Guid userId)
    {
        var rating = await Context.Ratings
            .FirstOrDefaultAsync(r => r.EntityId == guideId && r.UserId == userId);
        return rating?.Value;
    }
}
