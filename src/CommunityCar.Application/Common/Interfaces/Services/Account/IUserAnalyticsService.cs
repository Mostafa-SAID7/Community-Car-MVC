using CommunityCar.Application.Features.Analytics.DTOs;
using CommunityCar.Application.Features.Analytics.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services;

public interface IUserAnalyticsService
{
    // Activity Tracking
    Task TrackActivityAsync(TrackActivityRequest request);

    Task TrackInteractionAsync(Guid userId, string interactionType, string entityType, Guid entityId, string? metadata = null);
    Task<IEnumerable<AnalyticsActivityVM>> GetUserActivitiesAsync(Guid userId, int days = 30, int limit = 100);
    Task<UserActivityStatsVM> GetUserActivityStatsAsync(Guid userId, int days = 30);

    // Interest Tracking
    Task UpdateUserInterestsAsync(Guid userId, string category, string subCategory, string interestType, string interestValue, double scoreIncrement = 1.0, string? source = null);
    Task<IEnumerable<AnalyticsInterestVM>> GetUserInterestsAsync(Guid userId, string? category = null, int limit = 50);
    Task<IEnumerable<AnalyticsInterestVM>> GetTopUserInterestsAsync(Guid userId, int limit = 10);
    Task CleanupStaleInterestsAsync(Guid userId, int daysThreshold = 90);

    // Following System
    Task<bool> FollowUserAsync(Guid followerId, Guid followedUserId, string? reason = null);
    Task<bool> UnfollowUserAsync(Guid followerId, Guid followedUserId);
    Task<IEnumerable<AnalyticsFollowingVM>> GetUserFollowingAsync(Guid userId, bool activeOnly = true);
    Task<IEnumerable<AnalyticsFollowingVM>> GetUserFollowersAsync(Guid userId, bool activeOnly = true);
    Task<bool> IsFollowingAsync(Guid followerId, Guid followedUserId);
    Task<IEnumerable<AnalyticsSuggestionVM>> GetFollowSuggestionsAsync(Guid userId, int limit = 10);

    // Content Recommendations
    Task<IEnumerable<ContentRecommendationVM>> GetContentRecommendationsAsync(Guid userId, string? contentType = null, int limit = 20);
    Task<IEnumerable<AnalyticsSuggestionVM>> GetPeopleRecommendationsAsync(Guid userId, int limit = 10);

    // Analytics and Insights
    Task<UserEngagementStatsVM> GetUserEngagementStatsAsync(Guid userId, int days = 30);
    Task<IEnumerable<TrendingTopicVM>> GetTrendingTopicsAsync(int days = 7, int limit = 20);
    Task<IEnumerable<PopularContentVM>> GetPopularContentAsync(string contentType, int days = 7, int limit = 20);

    // Privacy and Data Management
    Task<bool> OptOutOfTrackingAsync(Guid userId);
    Task<bool> OptInToTrackingAsync(Guid userId);
    Task<bool> DeleteUserDataAsync(Guid userId);
    Task<UserPrivacySettingsVM> GetUserPrivacySettingsAsync(Guid userId);
    Task UpdateUserPrivacySettingsAsync(Guid userId, UpdateAnalyticsPrivacySettingsRequest request);
}


