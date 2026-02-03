using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Community.Posts;
using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Community;

public interface IPostsRepository : IBaseRepository<Post>
{
    Task<Post?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<Post?> GetBySlugIncludeDeletedAsync(string slug, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Post> Items, int TotalCount)> SearchAsync(
        string? searchTerm, 
        PostType? type, 
        Guid? authorId, 
        Guid? groupId, 
        string? sortBy, 
        int page, 
        int pageSize, 
        CancellationToken cancellationToken = default);
    Task<(IEnumerable<Post> Items, int TotalCount)> SearchIncludeDeletedAsync(
        string? searchTerm, 
        PostType? type, 
        Guid? authorId, 
        Guid? groupId, 
        string? sortBy, 
        int page, 
        int pageSize, 
        bool includeDeleted = false,
        bool deletedOnly = false,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetByAuthorAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetByAuthorIncludeDeletedAsync(Guid userId, bool includeDeleted = false, bool deletedOnly = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetByGroupAsync(Guid groupId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetByGroupIncludeDeletedAsync(Guid groupId, bool includeDeleted = false, bool deletedOnly = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetRecentPostsAsync(int count, CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetRecentPostsIncludeDeletedAsync(int count, bool includeDeleted = false, bool deletedOnly = false, CancellationToken cancellationToken = default);
    Task<int> GetUserPostsCountAsync(Guid userId);
    Task<int> GetUserPostsCountIncludeDeletedAsync(Guid userId, bool includeDeleted = false, bool deletedOnly = false);
    
    // Bulk operations for posts
    Task<int> SoftDeletePostsByAuthorAsync(Guid authorId, string? deletedBy = null);
    Task<int> SoftDeletePostsByGroupAsync(Guid groupId, string? deletedBy = null);
    Task<int> RestorePostsByAuthorAsync(Guid authorId, string? restoredBy = null);
    Task<int> RestorePostsByGroupAsync(Guid groupId, string? restoredBy = null);
    
    // Admin operations
    Task<IEnumerable<Post>> GetDeletedPostsForModerationAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<int> GetDeletedPostsCountAsync();
    
    // BroadcastHub specific methods
    Task<IEnumerable<Post>> GetAccessiblePostsAsync(Guid userId, int page = 1, int pageSize = 10, string? category = null, string? sortBy = "recent", CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetGroupPostsAsync(Guid groupId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
}
