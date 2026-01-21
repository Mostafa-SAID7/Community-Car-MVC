using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Domain.Enums;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Shared;

public class ViewRepository : BaseRepository<View>, IViewRepository
{
    public ViewRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<View>> GetViewsByEntityAsync(Guid entityId, EntityType entityType)
    {
        return await Context.Views
            .Where(v => v.EntityId == entityId && v.EntityType == entityType)
            .ToListAsync();
    }

    public async Task<int> GetViewCountAsync(Guid entityId, EntityType entityType)
    {
        return await Context.Views
            .CountAsync(v => v.EntityId == entityId && v.EntityType == entityType);
    }

    public async Task<bool> HasUserViewedAsync(Guid entityId, EntityType entityType, Guid userId)
    {
        return await Context.Views
            .AnyAsync(v => v.EntityId == entityId && 
                          v.EntityType == entityType && 
                          v.UserId == userId);
    }
}