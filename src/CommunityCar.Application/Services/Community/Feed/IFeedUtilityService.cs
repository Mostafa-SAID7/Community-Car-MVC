using CommunityCar.Application.Features.Community.Feed.ViewModels;
using CommunityCar.Application.Common.Models;
using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Services.Community.Feed;

/// <summary>
/// Service responsible for feed utility operations (sorting, pagination, stats)
/// </summary>
public interface IFeedUtilityService
{
    /// <summary>
    /// Gets user interests for personalization
    /// </summary>
    Task<List<string>> GetUserInterestsAsync(Guid? userId);

    /// <summary>
    /// Gets user's friend IDs
    /// </summary>
    Task<List<Guid>> GetUserFriendIdsAsync(Guid? userId);

    /// <summary>
    /// Gets active stories for the feed
    /// </summary>
    Task<IEnumerable<StoryFeedVM>> GetActiveStoriesAsync(Guid? userId = null);

    /// <summary>
    /// Gets trending topics
    /// </summary>
    Task<IEnumerable<TrendingTopicVM>> GetTrendingTopicsAsync(int count = 10);

    /// <summary>
    /// Gets suggested friends for the user
    /// </summary>
    Task<IEnumerable<SuggestedFriendVM>> GetSuggestedFriendsAsync(Guid userId, int count = 5);

    /// <summary>
    /// Gets feed statistics
    /// </summary>
    Task<FeedStatsVM> GetFeedStatsAsync(Guid? userId = null);

    /// <summary>
    /// Applies sorting to feed items
    /// </summary>
    List<FeedItemVM> ApplySorting(List<FeedItemVM> items, FeedSortBy sortBy);

    /// <summary>
    /// Applies pagination to feed items
    /// </summary>
    List<FeedItemVM> ApplyPagination(List<FeedItemVM> items, int page, int pageSize);

    /// <summary>
    /// Creates pagination information
    /// </summary>
    CommunityCar.Application.Common.Models.PaginationInfo CreatePaginationInfo(int currentPage, int pageSize, int totalItems);

    /// <summary>
    /// Calculates time remaining for stories
    /// </summary>
    string CalculateTimeRemaining(DateTime expiresAt);
    Task<int> CleanupExpiredStoriesAsync();
}


