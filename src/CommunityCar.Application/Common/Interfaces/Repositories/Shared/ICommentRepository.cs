using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Shared;

public interface ICommentRepository : IBaseRepository<Comment>
{
    Task<IEnumerable<Comment>> GetEntityCommentsAsync(Guid entityId, EntityType entityType);
    Task<int> GetCommentCountAsync(Guid entityId, EntityType entityType);
    Task<IEnumerable<Comment>> GetUserCommentsAsync(Guid userId, EntityType? entityType = null);
    Task<IEnumerable<Comment>> GetRecentCommentsAsync(int count);
    Task<IEnumerable<Comment>> GetRepliesAsync(Guid parentCommentId);
    Task<IEnumerable<Comment>> GetTopLevelCommentsAsync(Guid entityId, EntityType entityType);
    Task<IEnumerable<Comment>> GetCommentRepliesAsync(Guid commentId);
    Task<int> GetEntityCommentCountAsync(Guid entityId, EntityType entityType);
}