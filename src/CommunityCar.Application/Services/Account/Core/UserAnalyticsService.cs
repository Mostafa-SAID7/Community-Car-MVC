using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Common.Interfaces.Repositories.Account;
using CommunityCar.Application.Features.Account.ViewModels.Activity;
using CommunityCar.Application.Features.Account.ViewModels.Core;
using CommunityCar.Application.Features.Account.ViewModels.Social;
using CommunityCar.Application.Features.Shared.ViewModels;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.Account.Core;

public class UserAnalyticsService : IUserAnalyticsService
{
    private readonly IUserActivityRepository _activityRepository;
    private readonly IUserInterestRepository _interestRepository;
    private readonly IUserFollowingRepository _followingRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileViewRepository _profileViewRepository;
    private readonly ILogger<UserAnalyticsService> _logger;

    public UserAnalyticsService(
        IUserActivityRepository activityRepository,
        IUserInterestRepository interestRepository,
        IUserFollowingRepository followingRepository,
        IUserRepository userRepository,
        IUserProfileViewRepository profileViewRepository,
        ILogger<UserAnalyticsService> logger)
    {
        _activityRepository = activityRepository;
        _interestRepository = interestRepository;
        _followingRepository = followingRepository;
        _userRepository = userRepository;
        _profileViewRepository = profileViewRepository;
        _logger = logger;
    }

    #region Activity Analytics

    public async Task<ActivityAnalyticsVM> GetUserActivityAnalyticsAsync(Guid userId, DateTime? fromDate = null)
    {
        try
        {
            var totalActivities = await _activityRepository.GetActivityCountAsync(userId, fromDate);
            var activitiesByType = await _activityRepository.GetActivityCountByTypeAsync(userId, fromDate);
            var lastActivityDate = await _activityRepository.GetLastActivityDateAsync(userId);

            return new ActivityAnalyticsVM
            {
                UserId = userId,
                TotalActivities = totalActivities,
                ActivitiesByType = activitiesByType,
                LastActivityDate = lastActivityDate,
                ActivityTrends = new List<ActivityTrendVM>() // TODO: Implement trends
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting activity analytics for user {UserId}", userId);
            return new ActivityAnalyticsVM { UserId = userId };
        }
    }

    public async Task<UserStatisticsVM> GetUserStatisticsAsync(Guid userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return new UserStatisticsVM { UserId = userId };

            var followingCount = await _followingRepository.GetFollowingCountAsync(userId);
            var followerCount = await _followingRepository.GetFollowerCountAsync(userId);
            var achievementsCount = 0; // TODO: Get from achievement repository
            var badgesCount = 0; // TODO: Get from badge repository
            var galleryItemsCount = 0; // TODO: Get from gallery repository
            var lastActivityDate = await _activityRepository.GetLastActivityDateAsync(userId);

            return new UserStatisticsVM
            {
                UserId = userId,
                PostsCount = 0, // TODO: Get from posts repository
                CommentsCount = 0, // TODO: Get from comments repository
                LikesReceived = 0, // TODO: Get from reactions repository
                SharesReceived = 0, // TODO: Get from shares repository
                FollowersCount = followerCount,
                FollowingCount = followingCount,
                AchievementsCount = achievementsCount,
                BadgesCount = badgesCount,
                GalleryItemsCount = galleryItemsCount,
                JoinedDate = user.CreatedAt,
                DaysActive = (DateTime.UtcNow - user.CreatedAt).Days,
                LastActivityDate = lastActivityDate
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user statistics for user {UserId}", userId);
            return new UserStatisticsVM { UserId = userId };
        }
    }

    #endregion

    #region Following Analytics

    public async Task<FollowingAnalyticsVM> GetFollowingAnalyticsAsync(Guid userId)
    {
        try
        {
            var followingCount = await _followingRepository.GetFollowingCountAsync(userId);
            var followerCount = await _followingRepository.GetFollowerCountAsync(userId);
            var recentFollowers = await _followingRepository.GetRecentFollowersAsync(userId, 10);
            var recentFollowing = await _followingRepository.GetRecentFollowingAsync(userId, 10);
            var suggestedFollowing = await _followingRepository.GetSuggestedFollowingAsync(userId, 10);

            return new FollowingAnalyticsVM
            {
                UserId = userId,
                FollowingCount = followingCount,
                FollowerCount = followerCount,
                MutualFollowingCount = 0, // TODO: Calculate mutual following
                RecentFollowers = recentFollowers.Select(f => new NetworkUserVM
                {
                    Id = f.Id,
                    FollowerId = f.FollowerId,
                    FollowingId = f.FollowedUserId,
                    CreatedAt = f.CreatedAt
                }).ToList(),
                RecentFollowing = recentFollowing.Select(f => new NetworkUserVM
                {
                    Id = f.Id,
                    FollowerId = f.FollowerId,
                    FollowingId = f.FollowedUserId,
                    CreatedAt = f.CreatedAt
                }).ToList(),
                SuggestedFollowing = suggestedFollowing.Select(u => new UserSuggestionVM { UserId = u }).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting following analytics for user {UserId}", userId);
            return new FollowingAnalyticsVM { UserId = userId };
        }
    }

    #endregion

    #region Interest Analytics

    public async Task<InterestAnalyticsVM> GetInterestAnalyticsAsync(Guid userId)
    {
        try
        {
            var totalInterests = await _interestRepository.GetInterestCountAsync(userId);
            var topInterests = await _interestRepository.GetTopInterestsAsync(userId, 10);
            var recommendedInterests = await _interestRepository.GetRecommendedInterestsAsync(userId, 10);
            var similarUsers = await _interestRepository.GetUsersWithSimilarInterestsAsync(userId, 10);

            return new InterestAnalyticsVM
            {
                UserId = userId,
                TotalInterests = totalInterests,
                InterestsByCategory = new Dictionary<string, int>(), // TODO: Implement
                TopInterests = topInterests.Select(i => new ProfileInterestVM
                {
                    Id = i.Id,
                    UserId = i.UserId,
                    InterestId = i.Id,
                    InterestName = i.InterestValue,
                    Category = i.Category,
                    Priority = (int)i.Score,
                    CreatedAt = i.LastInteraction
                }).ToList(),
                RecommendedInterests = recommendedInterests.Select(id => id.ToString()).ToList(),
                SimilarUsers = similarUsers.Select(u => new UserSuggestionVM { UserId = u }).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting interest analytics for user {UserId}", userId);
            return new InterestAnalyticsVM { UserId = userId };
        }
    }

    #endregion

    #region Profile View Analytics

    public async Task<ProfileViewStatsVM> GetProfileViewAnalyticsAsync(Guid userId, DateTime? fromDate = null)
    {
        try
        {
            var totalViews = await _profileViewRepository.GetProfileViewCountAsync(userId, fromDate);
            var uniqueViewers = await _profileViewRepository.GetUniqueViewerCountAsync(userId, fromDate);
            var recentViews = await _profileViewRepository.GetRecentViewsAsync(userId, 10);
            var topViewers = await _profileViewRepository.GetTopViewersAsync(userId, 10);

            var fromDateValue = fromDate ?? DateTime.UtcNow.AddDays(-30);
            var toDate = DateTime.UtcNow;
            var viewsByDate = await _profileViewRepository.GetViewsByDateAsync(userId, fromDateValue, toDate);

            return new ProfileViewStatsVM
            {
                ProfileUserId = userId,
                TotalViews = totalViews,
                UniqueViewers = uniqueViewers,
                ViewsByDate = viewsByDate,
                RecentViews = recentViews.Select(v => new ProfileViewVM
                {
                    Id = v.Id,
                    ViewerId = v.ViewerId,
                    ProfileUserId = v.ProfileUserId,
                    ViewedAt = v.ViewedAt,
                    IsAnonymous = v.IsAnonymous
                }).ToList(),
                TopViewers = topViewers.Select(id => new ProfileViewerVM { ViewerId = id }).ToList(),
                AverageViewsPerDay = totalViews / Math.Max((toDate - fromDateValue).Days, 1)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting profile view analytics for user {UserId}", userId);
            return new ProfileViewStatsVM { ProfileUserId = userId };
        }
    }

    #endregion

    #region Activity Tracking

    public async Task<bool> LogUserActivityAsync(Guid userId, string activityType, string description, Dictionary<string, object>? metadata = null)
    {
        try
        {
            await _activityRepository.LogActivityAsync(userId, activityType, description, metadata);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging activity for user {UserId}", userId);
            return false;
        }
    }

    #endregion

    #region Data Management

    public async Task<bool> CleanupOldDataAsync(DateTime cutoffDate)
    {
        try
        {
            var activitiesDeleted = await _activityRepository.DeleteOldActivitiesAsync(cutoffDate);
            var viewsDeleted = await _profileViewRepository.CleanupOldViewsAsync(cutoffDate);

            _logger.LogInformation("Cleaned up old data: Activities={ActivitiesDeleted}, Views={ViewsDeleted}", 
                activitiesDeleted, viewsDeleted);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up old data");
            return false;
        }
    }

    #endregion

    #region User Statistics (extracted from IUserRepository)

    public async Task<int> GetTotalUsersCountAsync()
    {
        try
        {
            var allUsers = await _userRepository.GetAllAsync();
            return allUsers.Count();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total users count");
            return 0;
        }
    }

    public async Task<int> GetActiveUsersCountAsync()
    {
        try
        {
            var activeUsers = await _userRepository.GetActiveUsersAsync();
            return activeUsers.Count();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active users count");
            return 0;
        }
    }

    public async Task<int> GetNewUsersCountAsync(DateTime fromDate)
    {
        try
        {
            var allUsers = await _userRepository.GetAllAsync();
            return allUsers.Count(u => u.CreatedAt >= fromDate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting new users count");
            return 0;
        }
    }

    #endregion
}
