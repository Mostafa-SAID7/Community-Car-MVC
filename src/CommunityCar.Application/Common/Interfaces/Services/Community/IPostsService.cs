using CommunityCar.Application.Features.Posts.DTOs;
using CommunityCar.Application.Features.Posts.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Community;

public interface IPostsService
{
    Task<PostVM?> GetPostByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PostsSearchResponse> SearchPostsAsync(PostsSearchRequest request, CancellationToken cancellationToken = default);
    Task<PostVM> CreatePostAsync(CreatePostRequest request, CancellationToken cancellationToken = default);
    Task<PostVM> UpdatePostAsync(Guid id, UpdatePostRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeletePostAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PostVM>> GetUserPostsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PostVM>> GetGroupPostsAsync(Guid groupId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PostVM>> GetRecentPostsAsync(int count = 10, CancellationToken cancellationToken = default);
    Task<PostsStatsVM> GetPostsStatsAsync(CancellationToken cancellationToken = default);
}