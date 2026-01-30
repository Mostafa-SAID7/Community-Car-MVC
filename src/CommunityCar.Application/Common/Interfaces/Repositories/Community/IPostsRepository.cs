using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Community.Posts;
using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Community;

public interface IPostsRepository : IBaseRepository<Post>
{
    Task<(IEnumerable<Post> Items, int TotalCount)> SearchAsync(
        string? searchTerm, 
        PostType? type, 
        Guid? authorId, 
        Guid? groupId, 
        string? sortBy, 
        int page, 
        int pageSize, 
        CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetByAuthorAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetByGroupAsync(Guid groupId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetRecentPostsAsync(int count, CancellationToken cancellationToken = default);
    Task<int> GetUserPostsCountAsync(Guid userId);
}
