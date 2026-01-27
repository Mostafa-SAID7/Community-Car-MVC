using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Shared;

public interface IViewRepository : IBaseRepository<View>
{
    Task<int> GetEntityViewCountAsync(Guid entityId, EntityType entityType);
    Task<int> GetUniqueViewCountAsync(Guid entityId, EntityType entityType);
    Task<Dictionary<DateTime, int>> GetViewStatsAsync(Guid entityId, EntityType entityType, DateTime startDate);
    Task<IEnumerable<View>> GetRecentViewsAsync(Guid entityId, EntityType entityType, int count);
    Task<IEnumerable<View>> GetUserViewsAsync(Guid userId, EntityType? entityType = null);
    Task<IEnumerable<object>> GetMostViewedAsync(EntityType entityType, DateTime startDate, int count);
    Task<IEnumerable<View>> GetViewsByEntityAsync(Guid entityId, EntityType entityType);
}


