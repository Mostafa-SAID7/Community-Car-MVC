using AutoMapper;
using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Features.Posts.DTOs;
using CommunityCar.Application.Features.Posts.ViewModels;
using CommunityCar.Domain.Entities.Community.Posts;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Services.Community;

public class PostsService : IPostsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public PostsService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<PostVM?> GetPostByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(id);
        return post == null ? null : _mapper.Map<PostVM>(post);
    }

    public async Task<PostsSearchResponse> SearchPostsAsync(PostsSearchRequest request, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _unitOfWork.Posts.SearchAsync(
            request.SearchTerm,
            request.Type,
            request.AuthorId,
            request.GroupId,
            request.SortBy,
            request.Page,
            request.PageSize,
            cancellationToken);

        var summaryItems = _mapper.Map<IEnumerable<PostSummaryVM>>(items);

        return new PostsSearchResponse
        {
            Items = summaryItems,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<PostVM> CreatePostAsync(CreatePostRequest request, CancellationToken cancellationToken = default)
    {
        var currentUserIdString = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User must be authenticated");
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
            throw new UnauthorizedAccessException("Invalid user ID");

        var post = new Post(
            request.Title,
            request.Content,
            request.Type,
            currentUserId);

        await _unitOfWork.Posts.AddAsync(post);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PostVM>(post);
    }

    public async Task<PostVM> UpdatePostAsync(Guid id, UpdatePostRequest request, CancellationToken cancellationToken = default)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(id);
        if (post == null) throw new ArgumentException("Post not found");

        var currentUserIdString = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User must be authenticated");
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
            throw new UnauthorizedAccessException("Invalid user ID");

        // Check if user can edit (author or admin)
        if (post.AuthorId != currentUserId)
        {
            throw new UnauthorizedAccessException("You can only edit your own posts");
        }

        post.UpdateContent(request.Content);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PostVM>(post);
    }

    public async Task<bool> DeletePostAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(id);
        if (post == null) return false;

        var currentUserIdString = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User must be authenticated");
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
            throw new UnauthorizedAccessException("Invalid user ID");

        // Check if user can delete (author or admin)
        if (post.AuthorId != currentUserId)
        {
            throw new UnauthorizedAccessException("You can only delete your own posts");
        }

        await _unitOfWork.Posts.DeleteAsync(post);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IEnumerable<PostVM>> GetUserPostsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var posts = await _unitOfWork.Posts.GetByAuthorAsync(userId, cancellationToken);
        return _mapper.Map<IEnumerable<PostVM>>(posts);
    }

    public async Task<IEnumerable<PostVM>> GetGroupPostsAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        var posts = await _unitOfWork.Posts.GetByGroupAsync(groupId, cancellationToken);
        return _mapper.Map<IEnumerable<PostVM>>(posts);
    }

    public async Task<IEnumerable<PostVM>> GetRecentPostsAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        var posts = await _unitOfWork.Posts.GetRecentPostsAsync(count, cancellationToken);
        return _mapper.Map<IEnumerable<PostVM>>(posts);
    }

    public async Task<PostsStatsVM> GetPostsStatsAsync(CancellationToken cancellationToken = default)
    {
        var allPosts = await _unitOfWork.Posts.GetAllAsync();
        var postsList = allPosts.ToList();

        var recentPosts = await GetRecentPostsAsync(5, cancellationToken);

        var stats = new PostsStatsVM
        {
            TotalPosts = postsList.Count,
            TextPosts = postsList.Count(p => p.Type == PostType.Text),
            ImagePosts = postsList.Count(p => p.Type == PostType.Image),
            VideoPosts = postsList.Count(p => p.Type == PostType.Video),
            LinkPosts = postsList.Count(p => p.Type == PostType.Link),
            PollPosts = postsList.Count(p => p.Type == PostType.Poll),
            RecentPosts = _mapper.Map<List<PostSummaryVM>>(recentPosts),
            PostsByType = postsList
                .GroupBy(p => p.Type.ToString())
                .ToDictionary(g => g.Key, g => g.Count())
        };

        return stats;
    }
}