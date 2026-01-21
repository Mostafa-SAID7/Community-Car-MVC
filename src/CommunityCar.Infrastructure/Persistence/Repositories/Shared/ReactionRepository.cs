using Microsoft.EntityFrameworkCore;
using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Domain.Enums;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Shared;

public class ReactionRepository : BaseRepository<Reaction>, IReactionRepository
{
    public ReactionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Reaction?> GetUserReactionAsync(Guid entityId, EntityType entityType, Guid userId)
    {
        return await Context.Set<Reaction>()
            .FirstOrDefaultAsync(r => r.EntityId == entityId && 
                                    r.EntityType == entityType && 
                                    r.UserId == userId);
    }

    public async Task<List<Reaction>> GetEntityReactionsAsync(Guid entityId, EntityType entityType)
    {
        return await Context.Set<Reaction>()
            .Where(r => r.EntityId == entityId && r.EntityType == entityType)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<Dictionary<ReactionType, int>> GetReactionCountsAsync(Guid entityId, EntityType entityType)
    {
        return await Context.Set<Reaction>()
            .Where(r => r.EntityId == entityId && r.EntityType == entityType)
            .GroupBy(r => r.Type)
            .ToDictionaryAsync(g => g.Key, g => g.Count());
    }

    public async Task<int> GetTotalReactionCountAsync(Guid entityId, EntityType entityType)
    {
        return await Context.Set<Reaction>()
            .CountAsync(r => r.EntityId == entityId && r.EntityType == entityType);
    }

    public async Task<List<Reaction>> GetUserReactionsAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        return await Context.Set<Reaction>()
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task RemoveUserReactionAsync(Guid entityId, EntityType entityType, Guid userId)
    {
        var reaction = await GetUserReactionAsync(entityId, entityType, userId);
        if (reaction != null)
        {
            Context.Set<Reaction>().Remove(reaction);
        }
    }

    public async Task<bool> HasUserReactedAsync(Guid entityId, EntityType entityType, Guid userId)
    {
        return await Context.Set<Reaction>()
            .AnyAsync(r => r.EntityId == entityId && 
                          r.EntityType == entityType && 
                          r.UserId == userId);
    }
}