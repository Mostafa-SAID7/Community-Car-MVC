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

    public async Task<List<Share>> GetEntitySharesAsync(Guid entityId, EntityType entityType)
    {
        return await Context.Set<Share>()
            .Where(s => s.EntityId == entityId && s.EntityType == entityType)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetEntityShareCountAsync(Guid entityId, EntityType entityType)
    {
        return await Context.Set<Share>()
            .CountAsync(s => s.EntityId == entityId && s.EntityType == entityType);
    }

    public async Task<List<Share>> GetUserSharesAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        return await Context.Set<Share>()
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Dictionary<ShareType, int>> GetShareTypeCountsAsync(Guid entityId, EntityType entityType)
    {
        return await Context.Set<Share>()
            .Where(s => s.EntityId == entityId && s.EntityType == entityType)
            .GroupBy(s => s.ShareType)
            .ToDictionaryAsync(g => g.Key, g => g.Count());
    }

    public async Task<List<Share>> GetRecentSharesAsync(int count = 10)
    {
        return await Context.Set<Share>()
            .OrderByDescending(s => s.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<bool> HasUserSharedAsync(Guid entityId, EntityType entityType, Guid userId)
    {
        return await Context.Set<Share>()
            .AnyAsync(s => s.EntityId == entityId && 
                          s.EntityType == entityType && 
                          s.UserId == userId);
    }
}