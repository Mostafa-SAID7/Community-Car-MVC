using AutoMapper;
using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Features.Community.Posts.ViewModels;
using CommunityCar.Domain.Entities.Community.Posts;
using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Services.Community.Posts;

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

    public async Task<PostVM?> GetPostBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var post = await _unitOfWork.Posts.GetBySlugAsync(slug, cancellationToken);
        return post == null ? null : _mapper.Map<PostVM>(post);
    }

    public async Task<PostsSearchVM> SearchPostsAsync(PostsSearchVM request, CancellationToken cancellationToken = default)
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

        return new PostsSearchVM
        {
            Items = summaryItems.ToList(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<PostVM> CreatePostAsync(CreatePostVM request, CancellationToken cancellationToken = default)
    {
        var currentUserIdString = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User must be authenticated");
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
            throw new UnauthorizedAccessException("Invalid user ID");

        var post = new Post(
            request.Title,
            request.Content,
            request.Type,
            currentUserId);

        post.UpdateArabicContent(request.TitleAr, request.ContentAr);

        await _unitOfWork.Posts.AddAsync(post);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PostVM>(post);
    }

    public async Task<PostVM> UpdatePostAsync(Guid id, UpdatePostVM request, CancellationToken cancellationToken = default)
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
        post.UpdateArabicContent(request.TitleAr, request.ContentAr);

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

    // SOFT DELETE METHODS

    public async Task<bool> SoftDeletePostAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(id);
        if (post == null) return false;

        var currentUserIdString = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User must be authenticated");
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
            throw new UnauthorizedAccessException("Invalid user ID");

        // Check if user can delete (author or admin)
        if (post.AuthorId != currentUserId && !await IsUserAdminAsync(currentUserId))
        {
            throw new UnauthorizedAccessException("You can only delete your own posts");
        }

        var deletedBy = _currentUserService.UserName ?? currentUserId.ToString();
        var result = await _unitOfWork.Posts.SoftDeleteAsync(id, deletedBy);
        if (result)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return result;
    }

    public async Task<bool> RestorePostAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var post = await _unitOfWork.Posts.GetByIdIncludeDeletedAsync(id);
        if (post == null || !post.IsDeleted) return false;

        var currentUserIdString = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User must be authenticated");
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
            throw new UnauthorizedAccessException("Invalid user ID");

        // Check if user can restore (author or admin)
        if (post.AuthorId != currentUserId && !await IsUserAdminAsync(currentUserId))
        {
            throw new UnauthorizedAccessException("You can only restore your own posts");
        }

        var restoredBy = _currentUserService.UserName ?? currentUserId.ToString();
        var result = await _unitOfWork.Posts.RestoreAsync(id, restoredBy);
        if (result)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return result;
    }

    public async Task<bool> PermanentDeletePostAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var post = await _unitOfWork.Posts.GetByIdIncludeDeletedAsync(id);
        if (post == null) return false;

        var currentUserIdString = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User must be authenticated");
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
            throw new UnauthorizedAccessException("Invalid user ID");

        // Only admins can permanently delete
        if (!await IsUserAdminAsync(currentUserId))
        {
            throw new UnauthorizedAccessException("Only administrators can permanently delete posts");
        }

        var result = await _unitOfWork.Posts.PermanentDeleteAsync(id);
        if (result)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return result;
    }

    public async Task<PostsSearchVM> SearchDeletedPostsAsync(PostsSearchVM request, CancellationToken cancellationToken = default)
    {
        var currentUserIdString = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User must be authenticated");
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
            throw new UnauthorizedAccessException("Invalid user ID");

        // Only allow users to see their own deleted posts or admins to see all
        var authorId = await IsUserAdminAsync(currentUserId) ? request.AuthorId : currentUserId;

        var (items, totalCount) = await _unitOfWork.Posts.SearchIncludeDeletedAsync(
            request.SearchTerm,
            request.Type,
            authorId,
            request.GroupId,
            request.SortBy,
            request.Page,
            request.PageSize,
            includeDeleted: false,
            deletedOnly: true,
            cancellationToken);

        var summaryItems = _mapper.Map<IEnumerable<PostSummaryVM>>(items);

        return new PostsSearchVM
        {
            Items = summaryItems.ToList(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<IEnumerable<PostVM>> GetDeletedPostsByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var currentUserIdString = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User must be authenticated");
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
            throw new UnauthorizedAccessException("Invalid user ID");

        // Users can only see their own deleted posts, admins can see any user's deleted posts
        if (userId != currentUserId && !await IsUserAdminAsync(currentUserId))
        {
            throw new UnauthorizedAccessException("You can only view your own deleted posts");
        }

        var posts = await _unitOfWork.Posts.GetByAuthorIncludeDeletedAsync(userId, includeDeleted: false, deletedOnly: true, cancellationToken);
        return _mapper.Map<IEnumerable<PostVM>>(posts);
    }

    public async Task<int> BulkSoftDeletePostsByAuthorAsync(Guid authorId, CancellationToken cancellationToken = default)
    {
        var currentUserIdString = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User must be authenticated");
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
            throw new UnauthorizedAccessException("Invalid user ID");

        // Only admins can bulk delete posts by author
        if (!await IsUserAdminAsync(currentUserId))
        {
            throw new UnauthorizedAccessException("Only administrators can bulk delete posts");
        }

        var deletedBy = _currentUserService.UserName ?? currentUserId.ToString();
        var result = await _unitOfWork.Posts.SoftDeletePostsByAuthorAsync(authorId, deletedBy);
        if (result > 0)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return result;
    }

    public async Task<int> BulkRestorePostsByAuthorAsync(Guid authorId, CancellationToken cancellationToken = default)
    {
        var currentUserIdString = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User must be authenticated");
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
            throw new UnauthorizedAccessException("Invalid user ID");

        // Only admins can bulk restore posts by author
        if (!await IsUserAdminAsync(currentUserId))
        {
            throw new UnauthorizedAccessException("Only administrators can bulk restore posts");
        }

        var restoredBy = _currentUserService.UserName ?? currentUserId.ToString();
        var result = await _unitOfWork.Posts.RestorePostsByAuthorAsync(authorId, restoredBy);
        if (result > 0)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return result;
    }

    // EXISTING METHODS (unchanged)

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

    public async Task<PostsStatsVM> GetPostsStatsIncludeDeletedAsync(CancellationToken cancellationToken = default)
    {
        var currentUserIdString = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User must be authenticated");
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
            throw new UnauthorizedAccessException("Invalid user ID");

        // Only admins can see stats including deleted posts
        if (!await IsUserAdminAsync(currentUserId))
        {
            throw new UnauthorizedAccessException("Only administrators can view deleted posts statistics");
        }

        var allPosts = await _unitOfWork.Posts.GetAllIncludeDeletedAsync();
        var postsList = allPosts.ToList();
        var activePosts = postsList.Where(p => !p.IsDeleted).ToList();
        var deletedPosts = postsList.Where(p => p.IsDeleted).ToList();

        var recentPosts = await GetRecentPostsAsync(5, cancellationToken);

        var stats = new PostsStatsVM
        {
            TotalPosts = activePosts.Count,
            TextPosts = activePosts.Count(p => p.Type == PostType.Text),
            ImagePosts = activePosts.Count(p => p.Type == PostType.Image),
            VideoPosts = activePosts.Count(p => p.Type == PostType.Video),
            LinkPosts = activePosts.Count(p => p.Type == PostType.Link),
            PollPosts = activePosts.Count(p => p.Type == PostType.Poll),
            RecentPosts = _mapper.Map<List<PostSummaryVM>>(recentPosts),
            PostsByType = activePosts
                .GroupBy(p => p.Type.ToString())
                .ToDictionary(g => g.Key, g => g.Count())
        };

        // Add deleted posts statistics
        stats.PostsByType.Add("DeletedPosts", deletedPosts.Count);
        stats.PostsByType.Add("TotalIncludingDeleted", postsList.Count);

        return stats;
    }

    // Helper method to check if user is admin (implement based on your authorization system)
    private async Task<bool> IsUserAdminAsync(Guid userId)
    {
        // TODO: Implement based on your role/permission system
        // For now, return false until proper role checking is implemented
        return false;
    }
}