using Microsoft.EntityFrameworkCore;
using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Domain.Enums;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Shared;

public class CommentRepository : BaseRepository<Comment>, ICommentRepository
{
    public CommentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Comment>> GetEntityCommentsAsync(Guid entityId, EntityType entityType)
    {
        return await DbSet.Where(c => c.EntityId == entityId && c.EntityType == entityType)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetCommentCountAsync(Guid entityId, EntityType entityType)
    {
        return await DbSet.CountAsync(c => c.EntityId == entityId && c.EntityType == entityType);
    }

    public async Task<IEnumerable<Comment>> GetUserCommentsAsync(Guid userId, EntityType? entityType = null)
    {
        var query = DbSet.Where(c => c.AuthorId == userId);
        
        if (entityType.HasValue)
        {
            query = query.Where(c => c.EntityType == entityType.Value);
        }
        
        return await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
    }

    public async Task<IEnumerable<Comment>> GetRecentCommentsAsync(int count)
    {
        return await DbSet.OrderByDescending(c => c.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Comment>> GetRepliesAsync(Guid parentCommentId)
    {
        return await DbSet.Where(c => c.ParentCommentId == parentCommentId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Comment>> GetTopLevelCommentsAsync(Guid entityId, EntityType entityType, int page = 1, int pageSize = 10)
    {
        return await DbSet.Where(c => c.EntityId == entityId && 
                                    c.EntityType == entityType && 
                                    c.ParentCommentId == null)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalTopLevelCommentCountAsync(Guid entityId, EntityType entityType)
    {
        return await DbSet.CountAsync(c => c.EntityId == entityId && 
                                         c.EntityType == entityType && 
                                         c.ParentCommentId == null);
    }

    public async Task<IEnumerable<Comment>> GetCommentRepliesAsync(Guid commentId)
    {
        return await DbSet.Where(c => c.ParentCommentId == commentId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetEntityCommentCountAsync(Guid entityId, EntityType entityType)
    {
        return await DbSet.CountAsync(c => c.EntityId == entityId && c.EntityType == entityType);
    }

    public async Task<Comment?> GetCommentWithRepliesAsync(Guid commentId)
    {
        return await DbSet
            .Include(c => c.Replies)
            .FirstOrDefaultAsync(c => c.Id == commentId);
    }
}