using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Shared;

public interface IReactionRepository : IBaseRepository<Reaction>
{
    Task<Reaction?> GetUserReactionAsync(Guid entityId, EntityType entityType, Guid userId);
    Task<List<Reaction>> GetEntityReactionsAsync(Guid entityId, EntityType entityType);
    Task<Dictionary<ReactionType, int>> GetReactionCountsAsync(Guid entityId, EntityType entityType);
    Task<int> GetTotalReactionCountAsync(Guid entityId, EntityType entityType);
    Task<List<Reaction>> GetUserReactionsAsync(Guid userId, int page = 1, int pageSize = 20);
    Task RemoveUserReactionAsync(Guid entityId, EntityType entityType, Guid userId);
    Task<bool> HasUserReactedAsync(Guid entityId, EntityType entityType, Guid userId);
}