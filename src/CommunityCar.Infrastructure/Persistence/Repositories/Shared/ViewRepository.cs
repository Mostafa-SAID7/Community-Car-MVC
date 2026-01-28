using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Domain.Enums.Shared;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Shared;

public class ViewRepository : BaseRepository<View>, IViewRepository
{
    public ViewRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<int> GetEntityViewCountAsync(Guid entityId, EntityType entityType)
    {
        return await DbSet.CountAsync(v => v.EntityId == entityId && v.EntityType == entityType);
    }

    public async Task<int> GetUniqueViewCountAsync(Guid entityId, EntityType entityType)
    {
        return await DbSet.Where(v => v.EntityId == entityId && v.EntityType == entityType)
            .Select(v => v.UserId)
            .Distinct()
            .CountAsync();
    }

    public async Task<Dictionary<DateTime, int>> GetViewStatsAsync(Guid entityId, EntityType entityType, DateTime startDate)
    {
        var views = await DbSet.Where(v => v.EntityId == entityId && 
                                         v.EntityType == entityType && 
                                         v.CreatedAt >= startDate)
            .GroupBy(v => v.CreatedAt.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToListAsync();

        return views.ToDictionary(v => v.Date, v => v.Count);
    }

    public async Task<IEnumerable<View>> GetRecentViewsAsync(Guid entityId, EntityType entityType, int count)
    {
        return await DbSet.Where(v => v.EntityId == entityId && v.EntityType == entityType)
            .OrderByDescending(v => v.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<View>> GetUserViewsAsync(Guid userId, EntityType? entityType = null)
    {
        var query = DbSet.Where(v => v.UserId == userId);
        
        if (entityType.HasValue)
        {
            query = query.Where(v => v.EntityType == entityType.Value);
        }
        
        return await query.OrderByDescending(v => v.CreatedAt).ToListAsync();
    }

    public async Task<IEnumerable<object>> GetMostViewedAsync(EntityType entityType, DateTime startDate, int count)
    {
        var mostViewed = await DbSet.Where(v => v.EntityType == entityType && v.CreatedAt >= startDate)
            .GroupBy(v => v.EntityId)
            .Select(g => new { EntityId = g.Key, ViewCount = g.Count() })
            .OrderByDescending(x => x.ViewCount)
            .Take(count)
            .ToListAsync();

        return mostViewed.Cast<object>();
    }

    public async Task<IEnumerable<View>> GetViewsByEntityAsync(Guid entityId, EntityType entityType)
    {
        return await DbSet.Where(v => v.EntityId == entityId && v.EntityType == entityType)
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync();
    }
}
