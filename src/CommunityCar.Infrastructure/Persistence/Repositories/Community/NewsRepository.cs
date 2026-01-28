using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Domain.Entities.Community.News;
using CommunityCar.Domain.Enums;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Community;

public class NewsRepository : BaseRepository<NewsItem>, INewsRepository
{
    public NewsRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<NewsItem?> GetBySlugAsync(string slug)
    {
        return await Context.Set<NewsItem>()
            .FirstOrDefaultAsync(n => n.Slug == slug);
    }

    public async Task<IEnumerable<NewsItem>> GetPublishedAsync()
    {
        return await Context.Set<NewsItem>()
            .Where(n => n.IsPublished)
            .OrderByDescending(n => n.PublishedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<NewsItem>> GetFeaturedAsync()
    {
        return await Context.Set<NewsItem>()
            .Where(n => n.IsFeatured && n.IsPublished)
            .OrderByDescending(n => n.PublishedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<NewsItem>> GetPinnedAsync()
    {
        return await Context.Set<NewsItem>()
            .Where(n => n.IsPinned && n.IsPublished)
            .OrderByDescending(n => n.PublishedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<NewsItem>> GetByAuthorAsync(Guid authorId)
    {
        return await Context.Set<NewsItem>()
            .Where(n => n.AuthorId == authorId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<NewsItem>> GetByCategoryAsync(NewsCategory category)
    {
        return await Context.Set<NewsItem>()
            .Where(n => n.Category == category && n.IsPublished)
            .OrderByDescending(n => n.PublishedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<NewsItem>> GetByCarMakeAsync(string carMake)
    {
        return await Context.Set<NewsItem>()
            .Where(n => n.CarMake != null && n.CarMake.ToLower() == carMake.ToLower() && n.IsPublished)
            .OrderByDescending(n => n.PublishedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<NewsItem>> GetByTagAsync(string tag)
    {
        return await Context.Set<NewsItem>()
            .Where(n => n.Tags.Contains(tag.ToLower()) && n.IsPublished)
            .OrderByDescending(n => n.PublishedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetPopularTagsAsync(int count)
    {
        var allNews = await Context.Set<NewsItem>()
            .Where(n => n.IsPublished)
            .ToListAsync();

        return allNews
            .SelectMany(n => n.Tags)
            .GroupBy(t => t)
            .OrderByDescending(g => g.Count())
            .Take(count)
            .Select(g => g.Key)
            .ToList();
    }

    public async Task<IEnumerable<string>> GetAvailableCarMakesAsync()
    {
        return await Context.Set<NewsItem>()
            .Where(n => n.CarMake != null && n.IsPublished)
            .Select(n => n.CarMake!)
            .Distinct()
            .OrderBy(m => m)
            .ToListAsync();
    }

    public async Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null)
    {
        var query = Context.Set<NewsItem>().Where(n => n.Slug == slug);
        
        if (excludeId.HasValue)
            query = query.Where(n => n.Id != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task<int> GetCountByUserAndDateAsync(Guid userId, DateTime date)
    {
        return await Context.Set<NewsItem>()
            .CountAsync(n => n.AuthorId == userId && n.CreatedAt.Date == date.Date);
    }
}

