using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Shared;

public interface ICommentRepository : IBaseRepository<Comment>
{
    Task<List<Comment>> GetEntityCommentsAsync(Guid entityId, EntityType entityType, int page = 1, int pageSize = 20);
    Task<List<Comment>> GetCommentRepliesAsync(Guid parentCommentId);
    Task<int> GetEntityCommentCountAsync(Guid entityId, EntityType entityType);
    Task<List<Comment>> GetUserCommentsAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<List<Comment>> GetTopLevelCommentsAsync(Guid entityId, EntityType entityType, int page = 1, int pageSize = 20);
    Task<Comment?> GetCommentWithRepliesAsync(Guid commentId);
}