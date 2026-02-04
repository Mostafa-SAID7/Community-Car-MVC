using AutoMapper;
using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.SoftDelete;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Features.Community.Posts.ViewModels;
using CommunityCar.Application.Features.Community.Stories.ViewModels;
using CommunityCar.Application.Features.Community.Groups.ViewModels;
using CommunityCar.Application.Features.Dashboard.SoftDelete.ViewModels;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.Dashboard.SoftDelete;

public class SoftDeleteService : ISoftDeleteService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<SoftDeleteService> _logger;

    public SoftDeleteService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUserService,
        ILogger<SoftDeleteService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    #region Posts Operations

    public async Task<bool> SoftDeletePostAsync(Guid postId, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = await GetCurrentUserIdAsync();
            var deletedBy = _currentUserService.UserName ?? currentUserId.ToString();

            var result = await _unitOfWork.Posts.SoftDeleteAsync(postId, deletedBy);
            if (result)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Post {PostId} soft deleted by user {UserId}", postId, currentUserId);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error soft deleting post {PostId}", postId);
            throw;
        }
    }

    public async Task<bool> RestorePostAsync(Guid postId, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = await GetCurrentUserIdAsync();
            var restoredBy = _currentUserService.UserName ?? currentUserId.ToString();

            var result = await _unitOfWork.Posts.RestoreAsync(postId, restoredBy);
            if (result)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Post {PostId} restored by user {UserId}", postId, currentUserId);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring post {PostId}", postId);
            throw;
        }
    }

    public async Task<bool> PermanentDeletePostAsync(Guid postId, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = await GetCurrentUserIdAsync();
            
            // Only admins can permanently delete
            if (!await IsUserAdminAsync(currentUserId))
            {
                throw new UnauthorizedAccessException("Only administrators can permanently delete posts");
            }

            var result = await _unitOfWork.Posts.PermanentDeleteAsync(postId);
            if (result)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogWarning("Post {PostId} permanently deleted by admin {UserId}", postId, currentUserId);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error permanently deleting post {PostId}", postId);
            throw;
        }
    }

    public async Task<int> BulkSoftDeletePostsAsync(IEnumerable<Guid> postIds, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = await GetCurrentUserIdAsync();
            var deletedBy = _currentUserService.UserName ?? currentUserId.ToString();

            var result = await _unitOfWork.Posts.SoftDeleteRangeAsync(postIds, deletedBy);
            if (result > 0)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("{Count} posts bulk soft deleted by user {UserId}", result, currentUserId);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk soft deleting posts");
            throw;
        }
    }

    public async Task<int> BulkRestorePostsAsync(IEnumerable<Guid> postIds, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = await GetCurrentUserIdAsync();
            var restoredBy = _currentUserService.UserName ?? currentUserId.ToString();

            var result = await _unitOfWork.Posts.RestoreRangeAsync(postIds, restoredBy);
            if (result > 0)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("{Count} posts bulk restored by user {UserId}", result, currentUserId);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk restoring posts");
            throw;
        }
    }

    public async Task<PostsSearchVM> GetDeletedPostsAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = await GetCurrentUserIdAsync();
            
            // Users can only see their own deleted posts, admins can see all
            var authorId = await IsUserAdminAsync(currentUserId) ? (Guid?)null : currentUserId;

            var (items, totalCount) = await _unitOfWork.Posts.SearchIncludeDeletedAsync(
                searchTerm: null,
                type: null,
                authorId: authorId,
                groupId: null,
                sortBy: "DeletedAt",
                page: page,
                pageSize: pageSize,
                includeDeleted: false,
                deletedOnly: true,
                cancellationToken);

            var summaryItems = _mapper.Map<IEnumerable<PostSummaryVM>>(items);

            return new PostsSearchVM
            {
                Items = summaryItems.ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting deleted posts");
            throw;
        }
    }

    #endregion

    #region Stories Operations

    public async Task<bool> SoftDeleteStoryAsync(Guid storyId, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = await GetCurrentUserIdAsync();
            var deletedBy = _currentUserService.UserName ?? currentUserId.ToString();

            // TODO: Implement when Stories repository is available
            // var result = await _unitOfWork.Stories.SoftDeleteAsync(storyId, deletedBy);
            // if (result)
            // {
            //     await _unitOfWork.SaveChangesAsync(cancellationToken);
            //     _logger.LogInformation("Story {StoryId} soft deleted by user {UserId}", storyId, currentUserId);
            // }
            // return result;

            _logger.LogWarning("Story soft delete not implemented yet for story {StoryId}", storyId);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error soft deleting story {StoryId}", storyId);
            throw;
        }
    }

    public async Task<bool> RestoreStoryAsync(Guid storyId, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = await GetCurrentUserIdAsync();
            var restoredBy = _currentUserService.UserName ?? currentUserId.ToString();

            // TODO: Implement when Stories repository is available
            _logger.LogWarning("Story restore not implemented yet for story {StoryId}", storyId);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring story {StoryId}", storyId);
            throw;
        }
    }

    public async Task<bool> PermanentDeleteStoryAsync(Guid storyId, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = await GetCurrentUserIdAsync();
            
            if (!await IsUserAdminAsync(currentUserId))
            {
                throw new UnauthorizedAccessException("Only administrators can permanently delete stories");
            }

            // TODO: Implement when Stories repository is available
            _logger.LogWarning("Story permanent delete not implemented yet for story {StoryId}", storyId);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error permanently deleting story {StoryId}", storyId);
            throw;
        }
    }

    public async Task<int> BulkSoftDeleteStoriesAsync(IEnumerable<Guid> storyIds, CancellationToken cancellationToken = default)
    {
        // TODO: Implement when Stories repository is available
        _logger.LogWarning("Bulk story soft delete not implemented yet");
        return 0;
    }

    public async Task<int> BulkRestoreStoriesAsync(IEnumerable<Guid> storyIds, CancellationToken cancellationToken = default)
    {
        // TODO: Implement when Stories repository is available
        _logger.LogWarning("Bulk story restore not implemented yet");
        return 0;
    }

    public async Task<StoriesSearchVM> GetDeletedStoriesAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        // TODO: Implement when Stories repository is available
        _logger.LogWarning("Get deleted stories not implemented yet");
        return new StoriesSearchVM { Results = new List<StoryVM>(), TotalCount = 0, Page = page, PageSize = pageSize };
    }

    #endregion

    #region Groups Operations

    public async Task<bool> SoftDeleteGroupAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = await GetCurrentUserIdAsync();
            var deletedBy = _currentUserService.UserName ?? currentUserId.ToString();

            // TODO: Implement when Groups repository is available
            _logger.LogWarning("Group soft delete not implemented yet for group {GroupId}", groupId);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error soft deleting group {GroupId}", groupId);
            throw;
        }
    }

    public async Task<bool> RestoreGroupAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = await GetCurrentUserIdAsync();
            var restoredBy = _currentUserService.UserName ?? currentUserId.ToString();

            // TODO: Implement when Groups repository is available
            _logger.LogWarning("Group restore not implemented yet for group {GroupId}", groupId);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring group {GroupId}", groupId);
            throw;
        }
    }

    public async Task<bool> PermanentDeleteGroupAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = await GetCurrentUserIdAsync();
            
            if (!await IsUserAdminAsync(currentUserId))
            {
                throw new UnauthorizedAccessException("Only administrators can permanently delete groups");
            }

            // TODO: Implement when Groups repository is available
            _logger.LogWarning("Group permanent delete not implemented yet for group {GroupId}", groupId);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error permanently deleting group {GroupId}", groupId);
            throw;
        }
    }

    public async Task<int> BulkSoftDeleteGroupsAsync(IEnumerable<Guid> groupIds, CancellationToken cancellationToken = default)
    {
        // TODO: Implement when Groups repository is available
        _logger.LogWarning("Bulk group soft delete not implemented yet");
        return 0;
    }

    public async Task<int> BulkRestoreGroupsAsync(IEnumerable<Guid> groupIds, CancellationToken cancellationToken = default)
    {
        // TODO: Implement when Groups repository is available
        _logger.LogWarning("Bulk group restore not implemented yet");
        return 0;
    }

    public async Task<GroupsSearchVM> GetDeletedGroupsAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        // TODO: Implement when Groups repository is available
        _logger.LogWarning("Get deleted groups not implemented yet");
        return new GroupsSearchVM { Items = new List<GroupSummaryVM>(), TotalCount = 0, Page = page, PageSize = pageSize };
    }

    #endregion

    #region User Operations

    public async Task<int> SoftDeleteAllUserContentAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = await GetCurrentUserIdAsync();
            
            // Only admins can delete all user content
            if (!await IsUserAdminAsync(currentUserId))
            {
                throw new UnauthorizedAccessException("Only administrators can delete all user content");
            }

            var deletedBy = _currentUserService.UserName ?? currentUserId.ToString();
            var totalDeleted = 0;

            // Delete user's posts
            var deletedPosts = await _unitOfWork.Posts.SoftDeletePostsByAuthorAsync(userId, deletedBy);
            totalDeleted += deletedPosts;

            // TODO: Add other content types when repositories are available
            // var deletedStories = await _unitOfWork.Stories.SoftDeleteStoriesByAuthorAsync(userId, deletedBy);
            // var deletedGroups = await _unitOfWork.Groups.SoftDeleteGroupsByOwnerAsync(userId, deletedBy);
            // totalDeleted += deletedStories + deletedGroups;

            if (totalDeleted > 0)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogWarning("All content for user {UserId} soft deleted by admin {AdminId}. Total items: {Count}", 
                    userId, currentUserId, totalDeleted);
            }

            return totalDeleted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error soft deleting all content for user {UserId}", userId);
            throw;
        }
    }

    public async Task<int> RestoreAllUserContentAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = await GetCurrentUserIdAsync();
            
            // Only admins can restore all user content
            if (!await IsUserAdminAsync(currentUserId))
            {
                throw new UnauthorizedAccessException("Only administrators can restore all user content");
            }

            var restoredBy = _currentUserService.UserName ?? currentUserId.ToString();
            var totalRestored = 0;

            // Restore user's posts
            var restoredPosts = await _unitOfWork.Posts.RestorePostsByAuthorAsync(userId, restoredBy);
            totalRestored += restoredPosts;

            // TODO: Add other content types when repositories are available

            if (totalRestored > 0)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("All content for user {UserId} restored by admin {AdminId}. Total items: {Count}", 
                    userId, currentUserId, totalRestored);
            }

            return totalRestored;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring all content for user {UserId}", userId);
            throw;
        }
    }

    public async Task<int> PermanentDeleteAllUserContentAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = await GetCurrentUserIdAsync();
            
            // Only admins can permanently delete all user content
            if (!await IsUserAdminAsync(currentUserId))
            {
                throw new UnauthorizedAccessException("Only administrators can permanently delete all user content");
            }

            var totalDeleted = 0;

            // Get all deleted posts by user and permanently delete them
            var deletedPosts = await _unitOfWork.Posts.GetByAuthorIncludeDeletedAsync(userId, includeDeleted: false, deletedOnly: true, cancellationToken);
            var postIds = deletedPosts.Select(p => p.Id);
            var permanentlyDeletedPosts = await _unitOfWork.Posts.PermanentDeleteRangeAsync(postIds);
            totalDeleted += permanentlyDeletedPosts;

            // TODO: Add other content types when repositories are available

            if (totalDeleted > 0)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogWarning("All deleted content for user {UserId} permanently deleted by admin {AdminId}. Total items: {Count}", 
                    userId, currentUserId, totalDeleted);
            }

            return totalDeleted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error permanently deleting all content for user {UserId}", userId);
            throw;
        }
    }

    #endregion

    #region Admin Operations

    public async Task<SoftDeleteStatsVM> GetSoftDeleteStatsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = await GetCurrentUserIdAsync();
            
            if (!await IsUserAdminAsync(currentUserId))
            {
                throw new UnauthorizedAccessException("Only administrators can view soft delete statistics");
            }

            var deletedPostsCount = await _unitOfWork.Posts.GetDeletedPostsCountAsync();
            
            // TODO: Add other content types when repositories are available
            var deletedStoriesCount = 0; // await _unitOfWork.Stories.GetDeletedStoriesCountAsync();
            var deletedGroupsCount = 0; // await _unitOfWork.Groups.GetDeletedGroupsCountAsync();
            var deletedCommentsCount = 0; // await _unitOfWork.Comments.GetDeletedCommentsCountAsync();

            return new SoftDeleteStatsVM
            {
                DeletedPosts = deletedPostsCount,
                DeletedStories = deletedStoriesCount,
                DeletedGroups = deletedGroupsCount,
                DeletedComments = deletedCommentsCount,
                TotalDeleted = deletedPostsCount + deletedStoriesCount + deletedGroupsCount + deletedCommentsCount,
                LastCleanup = DateTime.UtcNow.AddDays(-7), // TODO: Store this in settings
                ItemsCleanedUpLastTime = 0 // TODO: Store this in settings
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting soft delete statistics");
            throw;
        }
    }

    public async Task<int> CleanupOldDeletedContentAsync(int daysOld = 30, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = await GetCurrentUserIdAsync();
            
            if (!await IsUserAdminAsync(currentUserId))
            {
                throw new UnauthorizedAccessException("Only administrators can cleanup old deleted content");
            }

            var cutoffDate = DateTime.UtcNow.AddDays(-daysOld);
            var totalCleaned = 0;

            // Get old deleted posts and permanently delete them
            var oldDeletedPosts = await _unitOfWork.Posts.GetDeletedOnlyAsync();
            var oldPostIds = oldDeletedPosts
                .Where(p => p.DeletedAt.HasValue && p.DeletedAt.Value < cutoffDate)
                .Select(p => p.Id);
            
            var cleanedPosts = await _unitOfWork.Posts.PermanentDeleteRangeAsync(oldPostIds);
            totalCleaned += cleanedPosts;

            // TODO: Add other content types when repositories are available

            if (totalCleaned > 0)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogWarning("Cleaned up {Count} old deleted items (older than {Days} days) by admin {AdminId}", 
                    totalCleaned, daysOld, currentUserId);
            }

            return totalCleaned;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up old deleted content");
            throw;
        }
    }

    public async Task<IEnumerable<DeletedContentSummaryVM>> GetRecentlyDeletedContentAsync(int days = 7, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = await GetCurrentUserIdAsync();
            
            if (!await IsUserAdminAsync(currentUserId))
            {
                throw new UnauthorizedAccessException("Only administrators can view recently deleted content");
            }

            var cutoffDate = DateTime.UtcNow.AddDays(-days);
            var recentlyDeleted = new List<DeletedContentSummaryVM>();

            // Get recently deleted posts
            var deletedPosts = await _unitOfWork.Posts.GetDeletedOnlyAsync();
            var recentDeletedPosts = deletedPosts
                .Where(p => p.DeletedAt.HasValue && p.DeletedAt.Value >= cutoffDate)
                .Select(p => new DeletedContentSummaryVM
                {
                    Id = p.Id,
                    Title = p.Title,
                    ContentType = "Post",
                    AuthorName = "Unknown", // TODO: Load from User entity
                    AuthorId = p.AuthorId,
                    DeletedAt = p.DeletedAt!.Value,
                    DeletedBy = p.DeletedBy,
                    CanRestore = true,
                    CanPermanentDelete = true
                });

            recentlyDeleted.AddRange(recentDeletedPosts);

            // TODO: Add other content types when repositories are available

            return recentlyDeleted.OrderByDescending(x => x.DeletedAt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recently deleted content");
            throw;
        }
    }

    #endregion

    #region Helper Methods

    private async Task<Guid> GetCurrentUserIdAsync()
    {
        var currentUserIdString = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User must be authenticated");
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
            throw new UnauthorizedAccessException("Invalid user ID");
        return currentUserId;
    }

    private async Task<bool> IsUserAdminAsync(Guid userId)
    {
        // TODO: Implement based on your role/permission system
        // For now, return false until proper role checking is implemented
        return false;
    }

    #endregion
}