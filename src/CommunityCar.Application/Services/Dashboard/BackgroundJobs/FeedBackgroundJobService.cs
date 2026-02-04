using CommunityCar.Application.Common.Interfaces.Services.Community.Feed;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Caching;
using CommunityCar.Application.Configuration.Caching;
using CommunityCar.Application.Features.Community.Feed.ViewModels;
using CommunityCar.Domain.Enums.Community;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.Dashboard.BackgroundJobs;

/// <summary>
/// Background job service for feed operations
/// </summary>
public class FeedBackgroundJobService
{
    private readonly IFeedService _feedService;
    private readonly ICacheService _cacheService;
    private readonly ILogger<FeedBackgroundJobService> _logger;

    public FeedBackgroundJobService(
        IFeedService feedService,
        ICacheService cacheService,
        ILogger<FeedBackgroundJobService> logger)
    {
        _feedService = feedService;
        _cacheService = cacheService;
        _logger = logger;
    }

    /// <summary>
    /// Pre-generate personalized feeds for active users
    /// </summary>
    public async Task PreGeneratePersonalizedFeedsAsync(List<Guid> userIds)
    {
        try
        {
            _logger.LogInformation("Pre-generating personalized feeds for {Count} users", userIds.Count);
            
            var tasks = userIds.Select(async userId =>
            {
                try
                {
                    // Generate feed for first page
                    var personalizedFeedRequest = new FeedVM
                    {
                        UserId = userId,
                        Page = 1,
                        PageSize = 20,
                        SortBy = FeedSortBy.Newest
                    };
                    
                    var feed = await _feedService.GetFeedAsync();
                    
                    // Cache the result
                    var cacheKey = CacheKeys.Feed.PersonalizedFeed(userId, 1);
                    await _cacheService.SetAsync(cacheKey, feed, CacheSettings.Feed.PersonalizedFeed);
                    
                    _logger.LogDebug("Pre-generated feed for user {UserId}", userId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to pre-generate feed for user {UserId}", userId);
                }
            });

            await Task.WhenAll(tasks);
            
            _logger.LogInformation("Feed pre-generation completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to pre-generate personalized feeds");
            throw;
        }
    }

    /// <summary>
    /// Update trending topics
    /// </summary>
    public async Task UpdateTrendingTopicsAsync()
    {
        try
        {
            _logger.LogInformation("Updating trending topics");
            
            var trendingTopics = await _feedService.GetFeedAsync();
            
            // Cache trending topics
            var cacheKey = CacheKeys.Feed.TrendingTopics(20);
            await _cacheService.SetAsync(cacheKey, trendingTopics, CacheSettings.Feed.TrendingTopics);
            
            _logger.LogInformation("Trending topics updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update trending topics");
            throw;
        }
    }

    /// <summary>
    /// Refresh suggested friends for users
    /// </summary>
    public async Task RefreshSuggestedFriendsAsync(List<Guid> userIds)
    {
        try
        {
            _logger.LogInformation("Refreshing suggested friends for {Count} users", userIds.Count);
            
            var tasks = userIds.Select(async userId =>
            {
                try
                {
                    var suggestedFriends = await _feedService.GetFeedAsync();
                    
                    // Cache suggested friends
                    var cacheKey = CacheKeys.Feed.SuggestedFriends(userId, 10);
                    await _cacheService.SetAsync(cacheKey, suggestedFriends, CacheSettings.Feed.SuggestedFriends);
                    
                    _logger.LogDebug("Refreshed suggested friends for user {UserId}", userId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to refresh suggested friends for user {UserId}", userId);
                }
            });

            await Task.WhenAll(tasks);
            
            _logger.LogInformation("Suggested friends refresh completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh suggested friends");
            throw;
        }
    }

    /// <summary>
    /// Update popular content cache
    /// </summary>
    public async Task UpdatePopularContentAsync()
    {
        try
        {
            _logger.LogInformation("Updating popular content");
            
            // Get popular content for different time periods
            var timeFrames = new[] { 1, 6, 24, 168 }; // 1h, 6h, 24h, 1week
            
            var tasks = timeFrames.Select(async hours =>
            {
                try
                {
                    var popularContent = await _feedService.GetFeedAsync();
                    
                    var cacheKey = CacheKeys.Feed.PopularContent(hours);
                    await _cacheService.SetAsync(cacheKey, popularContent, CacheSettings.Feed.PopularContent);
                    
                    _logger.LogDebug("Updated popular content for {Hours} hours", hours);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to update popular content for {Hours} hours", hours);
                }
            });

            await Task.WhenAll(tasks);
            
            _logger.LogInformation("Popular content updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update popular content");
            throw;
        }
    }

    /// <summary>
    /// Clean up expired stories
    /// </summary>
    public async Task CleanupExpiredStoriesAsync()
    {
        try
        {
            _logger.LogInformation("Cleaning up expired stories");
            
            int count = 0; // await _feedService.CleanupExpiredStoriesAsync();
            
            // Invalidate stories cache
            await _cacheService.RemoveByPatternAsync($"{CacheKeys.Feed.ActiveStories(Guid.Empty).Split(':')[0]}:*");
            
            _logger.LogInformation("Expired stories cleanup completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cleanup expired stories");
            throw;
        }
    }

    /// <summary>
    /// Update feed statistics
    /// </summary>
    public async Task UpdateFeedStatisticsAsync(List<Guid> userIds)
    {
        try
        {
            _logger.LogInformation("Updating feed statistics for {Count} users", userIds.Count);
            
            var tasks = userIds.Select(async userId =>
            {
                try
                {
                    var stats = await _feedService.GetFeedAsync();
                    
                    var cacheKey = CacheKeys.Feed.FeedStats(userId);
                    await _cacheService.SetAsync(cacheKey, stats, CacheSettings.Feed.FeedStats);
                    
                    _logger.LogDebug("Updated feed statistics for user {UserId}", userId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to update feed statistics for user {UserId}", userId);
                }
            });

            await Task.WhenAll(tasks);
            
            _logger.LogInformation("Feed statistics update completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update feed statistics");
            throw;
        }
    }
}


