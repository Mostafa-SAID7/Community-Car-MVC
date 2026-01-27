using CommunityCar.Domain.Entities.Community.Posts;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Community;

public interface IPostsRepository
{
    Task<Post?> GetByIdAsync(Guid id);
    Task<IEnumerable<Post>> GetAllAsync();
    Task<(IEnumerable<Post> Items, int TotalCount)> SearchAsync(
        string? searchTerm = null,
        PostType? type = null,
        Guid? authorId = null,
        Guid? groupId = null,
        string? sortBy = "newest",
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetByAuthorAsync(Guid authorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetByGroupAsync(Guid groupId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetRecentPostsAsync(int count = 10, CancellationToken cancellationToken = default);
    Task<int> GetUserPostsCountAsync(Guid userId);
    Task AddAsync(Post post);
    Task UpdateAsync(Post post);
    Task DeleteAsync(Post post);
}


