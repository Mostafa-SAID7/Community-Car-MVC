using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Caching;
using CommunityCar.Application.Common.Interfaces.Services.Community.Feed;
using CommunityCar.Application.Common.Interfaces.Services.Account.Gamification;
using CommunityCar.Application.Common.Interfaces.Services.Community.News;
using CommunityCar.Application.Common.Interfaces.Services.Community.Events;
using CommunityCar.Application.Configuration.Caching;
using CommunityCar.Application.Features.Community.Feed.ViewModels;
using CommunityCar.Domain.Enums.Community;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.Dashboard.Caching;

/// <summary>
/// Service for warming up cache with frequently accessed data
/// </summary>
public class CacheWarmupService
{
    private readonly ICacheService _cacheService;
    private readonly IFeedService _feedService;
    private readonly IGamificationService _gamificationService;
    private readonly INewsService _newsService;
    private readonly IEventsService _eventsService;
    private readonly ILogger<CacheWarmupService> _logger;

    public CacheWarmupService(
        ICacheService cacheService,
        IFeedService feedService,
        IGamificationService gamificationService,
        INewsService newsService,
        IEventsService eventsService,
        ILogger<CacheWarmupService> logger)
    {
        _cacheService = cacheService;
        _feedService = feedService;
        _gamificationService = gamificationService;
        _newsService = newsService;
        _eventsService = eventsService;
        _logger = logger;
    }

    /// <summary>
    /// Warm up all critical cache data
    /// </summary>
    public async Task WarmupAllCacheAsync()
    {
        try
        {
            _logger.LogInformation("Starting cache warmup");

            var tasks = new List<Task>
            {
                WarmupReferenceDataAsync(),
                WarmupTrendingContentAsync(),
                WarmupGamificationDataAsync(),
                WarmupCommunityDataAsync()
            };

            await Task.WhenAll(tasks);

            _logger.LogInformation("Cache warmup completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to warm up cache");
            throw;
        }
    }

    /// <summary>
    /// Warm up reference data (categories, tags, etc.)
    /// </summary>
    public async Task WarmupReferenceDataAsync()
    {
        try
        {
            _logger.LogInformation("Warming up reference data cache");

            // Categories
            var categoriesKey = CacheKeys.Reference.Categories();
            await _cacheService.GetOrSetAsync(categoriesKey, async () =>
            {
                // TODO: Get categories from repository
                return new List<object> { new { Id = 1, Name = "General" } };
            }, CacheSettings.Reference.Categories);

            // Tags
            var tagsKey = CacheKeys.Reference.Tags();
            await _cacheService.GetOrSetAsync(tagsKey, async () =>
            {
                // TODO: Get tags from repository
                return new List<object> { new { Id = 1, Name = "Popular" } };
            }, CacheSettings.Reference.Tags);

            // Car makes
            var carMakesKey = CacheKeys.Reference.CarMakes();
            await _cacheService.GetOrSetAsync(carMakesKey, async () =>
            {
                // TODO: Get car makes from repository
                return new List<object> 
                { 
                    new { Id = 1, Name = "Toyota" },
                    new { Id = 2, Name = "Honda" },
                    new { Id = 3, Name = "Ford" }
                };
            }, CacheSettings.Reference.CarMakes);

            _logger.LogDebug("Reference data cache warmed up");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to warm up reference data cache");
        }
    }

    /// <summary>
    /// Warm up trending content
    /// </summary>
    public async Task WarmupTrendingContentAsync()
    {
        try
        {
            _logger.LogInformation("Warming up trending content cache");

            // Trending topics
            var trendingTopicsKey = CacheKeys.Feed.TrendingTopics(20);
            await _cacheService.GetOrSetAsync(trendingTopicsKey, async () =>
            {
                return await _feedService.GetFeedAsync();
            }, CacheSettings.Feed.TrendingTopics);

            // Popular content for different time frames
            var timeFrames = new[] { 1, 6, 24, 168 }; // 1h, 6h, 24h, 1week
            var popularContentTasks = timeFrames.Select(async hours =>
            {
                var popularContentKey = CacheKeys.Feed.PopularContent(hours);
                await _cacheService.GetOrSetAsync(popularContentKey, async () =>
                {
                    return await _feedService.GetFeedAsync();
                }, CacheSettings.Feed.PopularContent);
            });

            await Task.WhenAll(popularContentTasks);

            _logger.LogDebug("Trending content cache warmed up");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to warm up trending content cache");
        }
    }

    /// <summary>
    /// Warm up gamification data
    /// </summary>
    public async Task WarmupGamificationDataAsync()
    {
        try
        {
            _logger.LogInformation("Warming up gamification data cache");

            // Available badges
            var availableBadgesKey = CacheKeys.Gamification.AvailableBadges();
            await _cacheService.GetOrSetAsync(availableBadgesKey, async () =>
            {
                return await _gamificationService.GetGamificationAsync();
            }, CacheSettings.Gamification.AvailableBadges);

            // Leaderboards
            var leaderboardCategories = new[] { "points", "badges", "achievements" };
            var leaderboardTasks = leaderboardCategories.Select(async category =>
            {
                var leaderboardKey = CacheKeys.Gamification.Leaderboard(category, 10);
                await _cacheService.GetOrSetAsync(leaderboardKey, async () =>
                {
                    return await _gamificationService.GetGamificationAsync();
                }, CacheSettings.Gamification.Leaderboard);
            });

            await Task.WhenAll(leaderboardTasks);

            _logger.LogDebug("Gamification data cache warmed up");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to warm up gamification data cache");
        }
    }

    /// <summary>
    /// Warm up community data
    /// </summary>
    public async Task WarmupCommunityDataAsync()
    {
        try
        {
            _logger.LogInformation("Warming up community data cache");

            // Recent news
            var newsCategories = new[] { "general", "automotive", "technology" };
            var newsTasks = newsCategories.Select(async category =>
            {
                var newsKey = CacheKeys.Community.News(category, 1);
                await _cacheService.GetOrSetAsync(newsKey, async () =>
                {
                    var request = new CommunityCar.Application.Features.Community.News.ViewModels.NewsSearchVM
                    {
                        Category = NewsCategory.General.ToString(),
                        PageNumber = 1,
                        PageSize = 10
                    };
                    return await _newsService.GetNewsAsync();
                }, CacheSettings.Community.News);
            });

            await Task.WhenAll(newsTasks);

            // Upcoming events
            var eventsKey = CacheKeys.Community.Events("all", DateTime.Today);
            await _cacheService.GetOrSetAsync(eventsKey, async () =>
            {
                var request = new CommunityCar.Application.Features.Community.Events.ViewModels.EventsSearchVM
                {
                    Location = "all",
                    StartDate = DateTime.Today,
                    Page = 1,
                    PageSize = 20
                };
                return await _eventsService.GetEventsAsync();
            }, CacheSettings.Community.Events);

            _logger.LogDebug("Community data cache warmed up");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to warm up community data cache");
        }
    }

    /// <summary>
    /// Warm up cache for specific user
    /// </summary>
    public async Task WarmupUserCacheAsync(Guid userId)
    {
        try
        {
            _logger.LogInformation("Warming up cache for user {UserId}", userId);

            var tasks = new List<Task>
            {
                WarmupUserProfileCacheAsync(userId),
                WarmupUserFeedCacheAsync(userId),
                WarmupUserGamificationCacheAsync(userId)
            };

            await Task.WhenAll(tasks);

            _logger.LogDebug("User cache warmed up for user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to warm up cache for user {UserId}", userId);
        }
    }

    /// <summary>
    /// Warm up user profile cache
    /// </summary>
    private async Task WarmupUserProfileCacheAsync(Guid userId)
    {
        try
        {
            // User friends
            var friendsKey = CacheKeys.User.Friends(userId);
            await _cacheService.GetOrSetAsync(friendsKey, async () =>
            {
                // TODO: Get user friends from service
                return new List<object>();
            }, CacheSettings.User.Friends);

            // User interests
            var interestsKey = CacheKeys.User.Interests(userId);
            await _cacheService.GetOrSetAsync(interestsKey, async () =>
            {
                // TODO: Get user interests from service
                return new List<object>();
            }, CacheSettings.User.Interests);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to warm up profile cache for user {UserId}", userId);
        }
    }

    /// <summary>
    /// Warm up user feed cache
    /// </summary>
    private async Task WarmupUserFeedCacheAsync(Guid userId)
    {
        try
        {
            // Personalized feed
            var feedKey = CacheKeys.Feed.PersonalizedFeed(userId, 1);
            await _cacheService.GetOrSetAsync(feedKey, async () =>
            {
                var request = new CommunityCar.Application.Features.Community.Feed.ViewModels.FeedVM
                {
                    UserId = userId,
                    Page = 1,
                    PageSize = 20,
                    SortBy = FeedSortBy.Newest
                };
                return await _feedService.GetFeedAsync();
            }, CacheSettings.Feed.PersonalizedFeed);

            // Suggested friends
            var suggestedFriendsKey = CacheKeys.Feed.SuggestedFriends(userId, 10);
            await _cacheService.GetOrSetAsync(suggestedFriendsKey, async () =>
            {
                return await _feedService.GetFeedAsync();
            }, CacheSettings.Feed.SuggestedFriends);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to warm up feed cache for user {UserId}", userId);
        }
    }

    /// <summary>
    /// Warm up user gamification cache
    /// </summary>
    private async Task WarmupUserGamificationCacheAsync(Guid userId)
    {
        try
        {
            // User badges
            var badgesKey = CacheKeys.Gamification.UserBadges(userId);
            await _cacheService.GetOrSetAsync(badgesKey, async () =>
            {
                return await _gamificationService.GetGamificationAsync();
            }, CacheSettings.Gamification.UserBadges);

            // User points
            var pointsKey = CacheKeys.Gamification.UserPoints(userId);
            await _cacheService.GetOrSetAsync(pointsKey, async () =>
            {
                return await _gamificationService.GetGamificationAsync();
            }, CacheSettings.Gamification.UserPoints);

            // User level
            var levelKey = CacheKeys.Gamification.UserLevel(userId);
            await _cacheService.GetOrSetAsync(levelKey, async () =>
            {
                return await _gamificationService.GetGamificationAsync();
            }, CacheSettings.Gamification.UserLevel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to warm up gamification cache for user {UserId}", userId);
        }
    }

    /// <summary>
    /// Invalidate cache for specific user
    /// </summary>
    public async Task InvalidateUserCacheAsync(Guid userId)
    {
        try
        {
            _logger.LogInformation("Invalidating cache for user {UserId}", userId);

            var tasks = new List<Task>
            {
                _cacheService.RemoveByPatternAsync(CacheKeys.Patterns.UserData(userId)),
                _cacheService.RemoveByPatternAsync(CacheKeys.Patterns.ProfileData(userId)),
                _cacheService.RemoveByPatternAsync(CacheKeys.Patterns.FeedData(userId)),
                _cacheService.RemoveByPatternAsync(CacheKeys.Patterns.GamificationData(userId))
            };

            await Task.WhenAll(tasks);

            _logger.LogDebug("Cache invalidated for user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to invalidate cache for user {UserId}", userId);
        }
    }
}


