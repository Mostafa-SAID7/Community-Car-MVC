using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Domain.Enums.Shared;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Shared;

public class BookmarkRepository : BaseRepository<Bookmark>, IBookmarkRepository
{
    public BookmarkRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Bookmark>> GetUserBookmarksAsync(Guid userId, EntityType? entityType = null)
    {
        var query = Context.Bookmarks.Where(b => b.UserId == userId);
        
        if (entityType.HasValue)
        {
            query = query.Where(b => b.EntityType == entityType.Value);
        }
        
        return await query.ToListAsync();
    }

    public async Task<Bookmark?> GetUserBookmarkAsync(Guid entityId, EntityType entityType, Guid userId)
    {
        return await Context.Bookmarks
            .FirstOrDefaultAsync(b => b.EntityId == entityId && 
                                    b.EntityType == entityType && 
                                    b.UserId == userId);
    }

    public async Task<bool> IsBookmarkedByUserAsync(Guid entityId, EntityType entityType, Guid userId)
    {
        return await Context.Bookmarks
            .AnyAsync(b => b.EntityId == entityId && 
                          b.EntityType == entityType && 
                          b.UserId == userId);
    }

    public async Task<int> GetBookmarkCountAsync(Guid entityId, EntityType entityType)
    {
        return await Context.Bookmarks
            .CountAsync(b => b.EntityId == entityId && b.EntityType == entityType);
    }
}
