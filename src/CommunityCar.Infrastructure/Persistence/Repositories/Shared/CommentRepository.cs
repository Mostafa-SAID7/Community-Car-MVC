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

    public async Task<List<Comment>> GetEntityCommentsAsync(Guid entityId, EntityType entityType, int page = 1, int pageSize = 20)
    {
        return await Context.Set<Comment>()
            .Where(c => c.EntityId == entityId && c.EntityType == entityType)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<Comment>> GetCommentRepliesAsync(Guid parentCommentId)
    {
        return await Context.Set<Comment>()
            .Where(c => c.ParentCommentId == parentCommentId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetEntityCommentCountAsync(Guid entityId, EntityType entityType)
    {
        return await Context.Set<Comment>()
            .CountAsync(c => c.EntityId == entityId && c.EntityType == entityType);
    }

    public async Task<List<Comment>> GetUserCommentsAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        return await Context.Set<Comment>()
            .Where(c => c.AuthorId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<Comment>> GetTopLevelCommentsAsync(Guid entityId, EntityType entityType, int page = 1, int pageSize = 20)
    {
        return await Context.Set<Comment>()
            .Where(c => c.EntityId == entityId && 
                       c.EntityType == entityType && 
                       c.ParentCommentId == null)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Comment?> GetCommentWithRepliesAsync(Guid commentId)
    {
        return await Context.Set<Comment>()
            .Include(c => c.Replies)
            .FirstOrDefaultAsync(c => c.Id == commentId);
    }
}