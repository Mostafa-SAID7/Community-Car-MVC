using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Shared;

public interface IVoteRepository : IBaseRepository<Vote>
{
    Task<Vote?> GetUserVoteAsync(Guid entityId, EntityType entityType, Guid userId);
    Task<int> GetVoteCountAsync(Guid entityId, EntityType entityType);
    Task<int> GetVoteCountAsync(Guid entityId, EntityType entityType, VoteType voteType);
    Task<int> GetVoteScoreAsync(Guid entityId, EntityType entityType);
    Task<IEnumerable<Vote>> GetVotesByEntityAsync(Guid entityId, EntityType entityType);
    Task<IEnumerable<Vote>> GetUserVotesAsync(Guid userId, EntityType? entityType = null);
}


