using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Domain.Entities.Community.News;
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
        return await Context.News
            .FirstOrDefaultAsync(n => n.Slug == slug);
    }

    public async Task<IEnumerable<NewsItem>> GetPublishedAsync()
    {
        return await Context.News
            .Where(n => n.IsPublished)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetCountByUserAndDateAsync(Guid userId, DateTime date)
    {
        return await Context.News
            .CountAsync(n => n.CreatedBy == userId.ToString() && n.CreatedAt.Date == date.Date);
    }

    public async Task<IEnumerable<string>> GetPopularTagsAsync(int count)
    {
        // For a stub, we'll return empty list or some defaults
        return await Task.FromResult(new List<string>());
    }

    public async Task<IEnumerable<string>> GetAvailableCarMakesAsync()
    {
        return await Task.FromResult(new List<string>());
    }
}
