using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Domain.Entities.Community.Stories;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Community;

public class StoriesRepository : BaseRepository<Story>, IStoriesRepository
{
    public StoriesRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Story?> GetBySlugAsync(string slug)
    {
        return await Context.Stories
            .FirstOrDefaultAsync(s => s.Slug == slug);
    }

    public async Task<IEnumerable<Story>> GetActiveAsync()
    {
        return await Context.Stories
            .Where(s => s.IsActive && !s.IsArchived)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Story>> GetTopActiveAsync(int count)
    {
        return await Context.Stories
            .Where(s => s.IsActive && !s.IsArchived)
            .OrderByDescending(s => s.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Story>> GetExpiredAsync()
    {
        return await Context.Stories
            .Where(s => !s.IsArchived && s.ExpiresAt < DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task<IEnumerable<Story>> GetByAuthorAsync(Guid authorId)
    {
        return await Context.Stories
            .Where(s => s.AuthorId == authorId)
            .ToListAsync();
    }

    public async Task DeleteExpiredAsync()
    {
        var expired = await Context.Stories
            .Where(s => s.ExpiresAt < DateTime.UtcNow)
            .ToListAsync();
        
        Context.Stories.RemoveRange(expired);
    }

    public async Task<IEnumerable<string>> GetPopularTagsAsync(int count)
    {
        return await Task.FromResult(new List<string>());
    }

    public async Task<IEnumerable<string>> GetAvailableCarMakesAsync()
    {
        return await Task.FromResult(new List<string>());
    }
}
