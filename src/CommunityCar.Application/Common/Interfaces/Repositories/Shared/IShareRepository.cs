using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Shared;

public interface IShareRepository : IBaseRepository<Share>
{
    Task<List<Share>> GetEntitySharesAsync(Guid entityId, EntityType entityType);
    Task<int> GetEntityShareCountAsync(Guid entityId, EntityType entityType);
    Task<List<Share>> GetUserSharesAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<Dictionary<ShareType, int>> GetShareTypeCountsAsync(Guid entityId, EntityType entityType);
    Task<List<Share>> GetRecentSharesAsync(int count = 10);
    Task<bool> HasUserSharedAsync(Guid entityId, EntityType entityType, Guid userId);
}