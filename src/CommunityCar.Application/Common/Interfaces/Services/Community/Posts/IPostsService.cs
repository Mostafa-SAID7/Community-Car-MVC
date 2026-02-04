using CommunityCar.Application.Features.Community.Posts.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Community.Posts;

public interface IPostsService
{
    Task<PostVM?> GetPostByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PostVM?> GetPostBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<PostsSearchVM> SearchPostsAsync(PostsSearchVM request, CancellationToken cancellationToken = default);
    Task<PostVM> CreatePostAsync(CreatePostVM request, CancellationToken cancellationToken = default);
    Task<PostVM> UpdatePostAsync(Guid id, UpdatePostVM request, CancellationToken cancellationToken = default);
    Task<bool> DeletePostAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> SoftDeletePostAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> RestorePostAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> PermanentDeletePostAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PostsSearchVM> SearchDeletedPostsAsync(PostsSearchVM request, CancellationToken cancellationToken = default);
    Task<IEnumerable<PostVM>> GetDeletedPostsByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<int> BulkSoftDeletePostsByAuthorAsync(Guid authorId, CancellationToken cancellationToken = default);
    Task<int> BulkRestorePostsByAuthorAsync(Guid authorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PostVM>> GetUserPostsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PostVM>> GetGroupPostsAsync(Guid groupId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PostVM>> GetRecentPostsAsync(int count = 10, CancellationToken cancellationToken = default);
    Task<PostsStatsVM> GetPostsStatsAsync(CancellationToken cancellationToken = default);
    Task<PostsStatsVM> GetPostsStatsIncludeDeletedAsync(CancellationToken cancellationToken = default);
}