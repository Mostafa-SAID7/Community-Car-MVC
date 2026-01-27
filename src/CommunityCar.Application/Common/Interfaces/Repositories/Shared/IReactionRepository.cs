using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Shared;

public interface IReactionRepository : IBaseRepository<Reaction>
{
    Task<Reaction?> GetUserReactionAsync(Guid entityId, EntityType entityType, Guid userId);
    Task<IEnumerable<Reaction>> GetEntityReactionsAsync(Guid entityId, EntityType entityType);
    Task<Dictionary<ReactionType, int>> GetReactionCountsAsync(Guid entityId, EntityType entityType);
    Task<IEnumerable<Reaction>> GetUserReactionsAsync(Guid userId, EntityType? entityType = null);
    Task RemoveUserReactionAsync(Guid entityId, EntityType entityType, Guid userId);
}


