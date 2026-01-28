using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Shared;

public interface IShareRepository : IBaseRepository<Share>
{
    Task<IEnumerable<Share>> GetEntitySharesAsync(Guid entityId, EntityType entityType);
    Task<int> GetShareCountAsync(Guid entityId, EntityType entityType);
    Task<IEnumerable<Share>> GetUserSharesAsync(Guid userId, EntityType? entityType = null);
    Task<IEnumerable<Share>> GetRecentSharesAsync(int count);
    Task<Dictionary<ShareType, int>> GetShareTypeCountsAsync(Guid entityId, EntityType entityType);
}


