using CommunityCar.Application.Features.Community.Posts.ViewModels;
using CommunityCar.Application.Features.Community.Stories.ViewModels;
using CommunityCar.Application.Features.Community.Groups.ViewModels;
using CommunityCar.Application.Features.Dashboard.SoftDelete.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.SoftDelete;

/// <summary>
/// Service for managing soft delete operations across community entities
/// </summary>
public interface ISoftDeleteService
{
    // Posts Soft Delete Operations
    Task<bool> SoftDeletePostAsync(Guid postId, CancellationToken cancellationToken = default);
    Task<bool> RestorePostAsync(Guid postId, CancellationToken cancellationToken = default);
    Task<bool> PermanentDeletePostAsync(Guid postId, CancellationToken cancellationToken = default);
    Task<int> BulkSoftDeletePostsAsync(IEnumerable<Guid> postIds, CancellationToken cancellationToken = default);
    Task<int> BulkRestorePostsAsync(IEnumerable<Guid> postIds, CancellationToken cancellationToken = default);
    Task<PostsSearchVM> GetDeletedPostsAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);

    // Stories Soft Delete Operations
    Task<bool> SoftDeleteStoryAsync(Guid storyId, CancellationToken cancellationToken = default);
    Task<bool> RestoreStoryAsync(Guid storyId, CancellationToken cancellationToken = default);
    Task<bool> PermanentDeleteStoryAsync(Guid storyId, CancellationToken cancellationToken = default);
    Task<int> BulkSoftDeleteStoriesAsync(IEnumerable<Guid> storyIds, CancellationToken cancellationToken = default);
    Task<int> BulkRestoreStoriesAsync(IEnumerable<Guid> storyIds, CancellationToken cancellationToken = default);
    Task<StoriesSearchVM> GetDeletedStoriesAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);

    // Groups Soft Delete Operations
    Task<bool> SoftDeleteGroupAsync(Guid groupId, CancellationToken cancellationToken = default);
    Task<bool> RestoreGroupAsync(Guid groupId, CancellationToken cancellationToken = default);
    Task<bool> PermanentDeleteGroupAsync(Guid groupId, CancellationToken cancellationToken = default);
    Task<int> BulkSoftDeleteGroupsAsync(IEnumerable<Guid> groupIds, CancellationToken cancellationToken = default);
    Task<int> BulkRestoreGroupsAsync(IEnumerable<Guid> groupIds, CancellationToken cancellationToken = default);
    Task<GroupsSearchVM> GetDeletedGroupsAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);

    // User-specific operations
    Task<int> SoftDeleteAllUserContentAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<int> RestoreAllUserContentAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<int> PermanentDeleteAllUserContentAsync(Guid userId, CancellationToken cancellationToken = default);

    // Admin operations
    Task<SoftDeleteStatsVM> GetSoftDeleteStatsAsync(CancellationToken cancellationToken = default);
    Task<int> CleanupOldDeletedContentAsync(int daysOld = 30, CancellationToken cancellationToken = default);
    Task<IEnumerable<DeletedContentSummaryVM>> GetRecentlyDeletedContentAsync(int days = 7, CancellationToken cancellationToken = default);
}