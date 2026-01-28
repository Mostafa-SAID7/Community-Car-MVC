using CommunityCar.Application.Common.Interfaces.Services;
using CommunityCar.Application.Common.Interfaces.Repositories.Profile;
using CommunityCar.Application.Common.Interfaces.Repositories.User;
using CommunityCar.Application.Features.Analytics.DTOs;
using CommunityCar.Application.Features.Analytics.ViewModels;
using CommunityCar.Domain.Entities.Account.Core;
using UserActivityEntity = CommunityCar.Domain.Entities.Account.Core.UserActivity;
using CommunityCar.Domain.Entities.Account.Profile;
using CommunityCar.Domain.Enums.Account;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.Analytics;

public class UserAnalyticsService : IUserAnalyticsService
{
    private readonly IUserActivityRepository _activityRepository;
    private readonly IUserInterestRepository _interestRepository;
    private readonly IUserFollowingRepository _followingRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserAnalyticsService> _logger;

    public UserAnalyticsService(
        IUserActivityRepository activityRepository,
        IUserInterestRepository interestRepository,
        IUserFollowingRepository followingRepository,
        IUserRepository userRepository,
        ILogger<UserAnalyticsService> logger)
    {
        _activityRepository = activityRepository;
        _interestRepository = interestRepository;
        _followingRepository = followingRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    #region Activity Tracking

    public async Task TrackActivityAsync(TrackActivityRequest request)
    {
        try
        {
            var activity = new UserActivityEntity(
                request.UserId,
                request.ActivityType,
                request.EntityType,
                request.EntityId,
                request.Description,
                request.Metadata,
                0);

            await _activityRepository.AddAsync(activity);
            _logger.LogDebug("Activity tracked for user {UserId}: {ActivityType}", request.UserId, request.ActivityType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking activity for user {UserId}", request.UserId);
        }
    }

    public async Task TrackViewAsync(Guid userId, string entityType, Guid entityId, string? entityTitle = null, int? duration = null)
    {
        var metadata = duration.HasValue ? $"{{\"duration\": {duration}}}" : null;
        await TrackActivityAsync(new TrackActivityRequest
        {
            UserId = userId,
            ActivityType = ActivityType.View,
            EntityType = entityType,
            EntityId = entityId,
            Description = $"Viewed {entityType}: {entityTitle ?? entityId.ToString()}",
            Metadata = metadata
        });
    }

    public async Task TrackInteractionAsync(Guid userId, string interactionType, string entityType, Guid entityId, string? metadata = null)
    {
        await TrackActivityAsync(new TrackActivityRequest
        {
            UserId = userId,
            ActivityType = ActivityType.Other,
            EntityType = entityType,
            EntityId = entityId,
            Description = $"{interactionType} on {entityType}",
            Metadata = metadata
        });
    }

    public async Task<IEnumerable<UserActivityVM>> GetUserActivitiesAsync(Guid userId, int days = 30, int limit = 100)
    {
        var activities = await _activityRepository.GetUserActivitiesAsync(userId, limit);
        return activities.Select(a => new UserActivityVM
        {
            Id = a.Id,
            UserId = a.UserId,
            ActivityType = a.ActivityType,
            EntityType = a.EntityType,
            EntityId = a.EntityId,
            Description = a.Description,
            ActivityDate = a.ActivityDate
        });
    }

    public async Task<UserActivityStatsVM> GetUserActivityStatsAsync(Guid userId, int days = 30)
    {
        var activities = await _activityRepository.GetUserActivitiesAsync(userId, 1000);
        var recentActivities = activities.Where(a => a.ActivityDate >= DateTime.UtcNow.AddDays(-days));

        return new UserActivityStatsVM
        {
            TotalActivities = recentActivities.Count(),
            ViewsCount = recentActivities.Count(a => a.ActivityType == ActivityType.View),
            LikesCount = recentActivities.Count(a => a.ActivityType == ActivityType.Like),
            CommentsCount = recentActivities.Count(a => a.ActivityType == ActivityType.Comment),
            SharesCount = recentActivities.Count(a => a.ActivityType == ActivityType.Share),
            SearchesCount = recentActivities.Count(a => a.ActivityType == ActivityType.Search),
            ActiveDays = recentActivities.GroupBy(a => a.ActivityDate.Date).Count(),
            LastActivity = recentActivities.OrderByDescending(a => a.ActivityDate).FirstOrDefault()?.ActivityDate,
            MostActiveDay = recentActivities.GroupBy(a => a.ActivityDate.DayOfWeek.ToString())
                .OrderByDescending(g => g.Count())
                .FirstOrDefault()?.Key ?? "Monday",
            ActivityBreakdown = recentActivities.GroupBy(a => a.ActivityType.ToString())
                .ToDictionary(g => g.Key, g => g.Count()),
            DailyActivity = recentActivities.GroupBy(a => a.ActivityDate.Date.ToString("yyyy-MM-dd"))
                .ToDictionary(g => g.Key, g => g.Count())
        };
    }

    #endregion

    #region Interest Tracking

    public async Task UpdateUserInterestsAsync(Guid userId, string category, string subCategory, string interestType, string interestValue, double scoreIncrement = 1.0, string? source = null)
    {
        try
        {
            // Simple implementation - just log for now
            _logger.LogDebug("Interest update for user {UserId}: {Category}/{InterestType}/{InterestValue}", userId, category, interestType, interestValue);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user interests for user {UserId}", userId);
        }
    }

    public async Task<IEnumerable<UserInterestVM>> GetUserInterestsAsync(Guid userId, string? category = null, int limit = 50)
    {
        // Return empty list for now
        return new List<UserInterestVM>();
    }

    public async Task<IEnumerable<UserInterestVM>> GetTopUserInterestsAsync(Guid userId, int limit = 10)
    {
        // Return empty list for now
        return new List<UserInterestVM>();
    }

    public async Task CleanupStaleInterestsAsync(Guid userId, int daysThreshold = 90)
    {
        await Task.CompletedTask;
    }

    #endregion

    #region Following System

    public async Task<bool> FollowUserAsync(Guid followerId, Guid followedUserId, string? reason = null)
    {
        if (followerId == followedUserId) return false;

        try
        {
            var following = new UserFollowing(followerId, followedUserId, reason);
            await _followingRepository.AddAsync(following);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error following user {FollowedUserId} by {FollowerId}", followedUserId, followerId);
            return false;
        }
    }

    public async Task<bool> UnfollowUserAsync(Guid followerId, Guid followedUserId)
    {
        try
        {
            var following = await _followingRepository.GetFollowingAsync(followerId, followedUserId);
            if (following == null) return false;

            await _followingRepository.DeleteAsync(following);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unfollowing user {FollowedUserId} by {FollowerId}", followedUserId, followerId);
            return false;
        }
    }

    public async Task<IEnumerable<UserFollowingVM>> GetUserFollowingAsync(Guid userId, bool activeOnly = true)
    {
        try
        {
            var following = await _followingRepository.GetUserFollowingAsync(userId, activeOnly);
            return following.Select(f => new UserFollowingVM
            {
                FollowerId = f.FollowerId,
                FollowedUserId = f.FollowedUserId,
                FollowedAt = f.FollowedAt,
                FollowReason = f.FollowReason
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting following list for user {UserId}", userId);
            return new List<UserFollowingVM>();
        }
    }

    public async Task<IEnumerable<UserFollowingVM>> GetUserFollowersAsync(Guid userId, bool activeOnly = true)
    {
        try
        {
            var followers = await _followingRepository.GetUserFollowersAsync(userId, activeOnly);
            return followers.Select(f => new UserFollowingVM
            {
                FollowerId = f.FollowerId,
                FollowedUserId = f.FollowedUserId,
                FollowedAt = f.FollowedAt,
                FollowReason = f.FollowReason
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting followers list for user {UserId}", userId);
            return new List<UserFollowingVM>();
        }
    }

    public async Task<bool> IsFollowingAsync(Guid followerId, Guid followedUserId)
    {
        try
        {
            var following = await _followingRepository.GetFollowingAsync(followerId, followedUserId);
            return following?.IsActive == true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {FollowerId} is following {FollowedUserId}", followerId, followedUserId);
            return false;
        }
    }

    public async Task<IEnumerable<UserSuggestionVM>> GetFollowSuggestionsAsync(Guid userId, int limit = 10)
    {
        try
        {
            var users = await _userRepository.GetActiveUsersAsync();
            var usersList = users.Take(limit * 2).ToList();
            var following = await _followingRepository.GetUserFollowingAsync(userId, true);
            var followingIds = following.Select(f => f.FollowedUserId).ToHashSet();
            followingIds.Add(userId);

            return usersList.Where(u => !followingIds.Contains(u.Id))
                .Take(limit)
                .Select(u => new UserSuggestionVM
                {
                    UserId = u.Id,
                    Name = u.Profile.FullName ?? u.UserName ?? "",
                    Avatar = u.Profile.ProfilePictureUrl,
                    SuggestionReason = "Popular user",
                    RelevanceScore = 0.8,
                    MutualFriendsCount = 0,
                    CommonInterests = new List<string>(),
                    IsVerified = false,
                    Location = null,
                    FollowersCount = 0,
                    PostsCount = 0
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting follow suggestions for user {UserId}", userId);
            return new List<UserSuggestionVM>();
        }
    }

    #endregion

    #region Content Recommendations

    public async Task<IEnumerable<ContentRecommendationVM>> GetContentRecommendationsAsync(Guid userId, string? contentType = null, int limit = 20)
    {
        return new List<ContentRecommendationVM>();
    }

    public async Task<IEnumerable<UserSuggestionVM>> GetPeopleRecommendationsAsync(Guid userId, int limit = 10)
    {
        return await GetFollowSuggestionsAsync(userId, limit);
    }

    #endregion

    #region Analytics and Insights

    public async Task<UserEngagementStatsVM> GetUserEngagementStatsAsync(Guid userId, int days = 30)
    {
        try
        {
            var activities = await _activityRepository.GetUserActivitiesAsync(userId, 1000);
            var recentActivities = activities.Where(a => a.ActivityDate >= DateTime.UtcNow.AddDays(-days));

            return new UserEngagementStatsVM
            {
                TotalPosts = recentActivities.Count(a => a.ActivityType == ActivityType.Post),
                TotalLikes = recentActivities.Count(a => a.ActivityType == ActivityType.Like),
                TotalComments = recentActivities.Count(a => a.ActivityType == ActivityType.Comment),
                TotalShares = recentActivities.Count(a => a.ActivityType == ActivityType.Share),
                TotalViews = recentActivities.Count(a => a.ActivityType == ActivityType.View),
                EngagementRate = recentActivities.Count() / (double)Math.Max(days, 1),
                ActiveDays = recentActivities.GroupBy(a => a.ActivityDate.Date).Count(),
                LastActiveAt = recentActivities.OrderByDescending(a => a.ActivityDate).FirstOrDefault()?.ActivityDate,
                EngagementLevel = "Medium",
                ContentTypeBreakdown = recentActivities.GroupBy(a => a.EntityType)
                    .ToDictionary(g => g.Key, g => g.Count()),
                EngagementTrends = new Dictionary<string, double> { { "weekly", 1.0 } }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting engagement stats for user {UserId}", userId);
            return new UserEngagementStatsVM();
        }
    }

    public async Task<IEnumerable<TrendingTopicVM>> GetTrendingTopicsAsync(int days = 7, int limit = 20)
    {
        return new List<TrendingTopicVM>();
    }

    public async Task<IEnumerable<PopularContentVM>> GetPopularContentAsync(string contentType, int days = 7, int limit = 20)
    {
        return new List<PopularContentVM>();
    }

    #endregion

    #region Privacy and Data Management

    public async Task<bool> OptOutOfTrackingAsync(Guid userId)
    {
        return true;
    }

    public async Task<bool> OptInToTrackingAsync(Guid userId)
    {
        return true;
    }

    public async Task<bool> DeleteUserDataAsync(Guid userId)
    {
        try
        {
            // Simple implementation for now
            _logger.LogInformation("Data deletion requested for user {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user data for user {UserId}", userId);
            return false;
        }
    }

    public async Task<UserPrivacySettingsVM> GetUserPrivacySettingsAsync(Guid userId)
    {
        return new UserPrivacySettingsVM
        {
            UserId = userId,
            AllowActivityTracking = true,
            AllowInterestTracking = true,
            AllowLocationTracking = false,
            AllowPersonalizedRecommendations = true,
            AllowDataSharing = false,
            AllowAnalytics = true,
            LastUpdated = DateTime.UtcNow
        };
    }

    public async Task UpdateUserPrivacySettingsAsync(Guid userId, UpdateAnalyticsPrivacySettingsRequest request)
    {
        await Task.CompletedTask;
    }

    #endregion
}