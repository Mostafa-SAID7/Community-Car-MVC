using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Shared;

public interface IViewRepository : IBaseRepository<View>
{
    Task<IEnumerable<View>> GetViewsByEntityAsync(Guid entityId, EntityType entityType);
    Task<int> GetViewCountAsync(Guid entityId, EntityType entityType);
    Task<bool> HasUserViewedAsync(Guid entityId, EntityType entityType, Guid userId);
}