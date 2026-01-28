using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Shared;

public interface IBookmarkRepository : IBaseRepository<Bookmark>
{
    Task<IEnumerable<Bookmark>> GetUserBookmarksAsync(Guid userId, EntityType? entityType = null);
    Task<Bookmark?> GetUserBookmarkAsync(Guid entityId, EntityType entityType, Guid userId);
    Task<bool> IsBookmarkedByUserAsync(Guid entityId, EntityType entityType, Guid userId);
    Task<int> GetBookmarkCountAsync(Guid entityId, EntityType entityType);
}


