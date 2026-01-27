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
        return await DbSet
            .FirstOrDefaultAsync(r => r.EntityId == entityId && 
                                    r.EntityType == entityType && 
                                    r.UserId == userId);
    }

    public async Task<IEnumerable<Reaction>> GetEntityReactionsAsync(Guid entityId, EntityType entityType)
    {
        return await DbSet
            .Where(r => r.EntityId == entityId && r.EntityType == entityType)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<Dictionary<ReactionType, int>> GetReactionCountsAsync(Guid entityId, EntityType entityType)
    {
        return await DbSet
            .Where(r => r.EntityId == entityId && r.EntityType == entityType)
            .GroupBy(r => r.Type)
            .ToDictionaryAsync(g => g.Key, g => g.Count());
    }

    public async Task<IEnumerable<Reaction>> GetUserReactionsAsync(Guid userId, EntityType? entityType = null)
    {
        var query = DbSet.Where(r => r.UserId == userId);
        
        if (entityType.HasValue)
        {
            query = query.Where(r => r.EntityType == entityType.Value);
        }
        
        return await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
    }

    public async Task RemoveUserReactionAsync(Guid entityId, EntityType entityType, Guid userId)
    {
        var reaction = await DbSet.FirstOrDefaultAsync(r => 
            r.EntityId == entityId && 
            r.EntityType == entityType && 
            r.UserId == userId);
            
        if (reaction != null)
        {
            DbSet.Remove(reaction);
            // Don't call SaveChangesAsync here - let UnitOfWork handle it
        }
    }
}
