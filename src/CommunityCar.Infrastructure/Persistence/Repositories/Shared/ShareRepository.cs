using Microsoft.EntityFrameworkCore;
using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Domain.Enums;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Shared;

public class ShareRepository : BaseRepository<Share>, IShareRepository
{
    public ShareRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Share>> GetEntitySharesAsync(Guid entityId, EntityType entityType)
    {
        return await DbSet
            .Where(s => s.EntityId == entityId && s.EntityType == entityType)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetShareCountAsync(Guid entityId, EntityType entityType)
    {
        return await DbSet.CountAsync(s => s.EntityId == entityId && s.EntityType == entityType);
    }

    public async Task<IEnumerable<Share>> GetUserSharesAsync(Guid userId, EntityType? entityType = null)
    {
        var query = DbSet.Where(s => s.UserId == userId);
        
        if (entityType.HasValue)
        {
            query = query.Where(s => s.EntityType == entityType.Value);
        }
        
        return await query.OrderByDescending(s => s.CreatedAt).ToListAsync();
    }

    public async Task<IEnumerable<Share>> GetRecentSharesAsync(int count)
    {
        return await DbSet
            .OrderByDescending(s => s.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Dictionary<ShareType, int>> GetShareTypeCountsAsync(Guid entityId, EntityType entityType)
    {
        var shares = await DbSet.Where(s => s.EntityId == entityId && s.EntityType == entityType)
            .GroupBy(s => s.ShareType)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToListAsync();

        return shares.ToDictionary(s => s.Type, s => s.Count);
    }
}