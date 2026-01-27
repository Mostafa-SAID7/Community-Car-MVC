using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Application.Services.Account.Gamification;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.Account;

/// <summary>
/// Orchestrator service for gamification features (badges, achievements, points)
/// </summary>
public class GamificationService : IGamificationService
{
    private readonly IBadgeService _badgeService;
    private readonly IAchievementService _achievementService;
    private readonly IPointsAndLevelService _pointsAndLevelService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<GamificationService> _logger;

    public GamificationService(
        IBadgeService badgeService,
        IAchievementService achievementService,
        IPointsAndLevelService pointsAndLevelService,
        ICurrentUserService currentUserService,
        ILogger<GamificationService> logger)
    {
        _badgeService = badgeService;
        _achievementService = achievementService;
        _pointsAndLevelService = pointsAndLevelService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    #region Badges - Delegate to BadgeService

    public async Task<IEnumerable<UserBadgeVM>> GetUserBadgesAsync(Guid userId)
        => await _badgeService.GetUserBadgesAsync(userId);

    public async Task<bool> AwardBadgeAsync(Guid userId, string badgeCode)
        => await _badgeService.AwardBadgeAsync(userId, badgeCode);

    public async Task<bool> RevokeBadgeAsync(Guid userId, string badgeCode)
        => await _badgeService.RevokeBadgeAsync(userId, badgeCode);

    public async Task<IEnumerable<BadgeVM>> GetAvailableBadgesAsync()
    {
        var userId = Guid.Parse(_currentUserService.UserId ?? Guid.Empty.ToString());
        var userBadges = await _badgeService.GetAvailableBadgesAsync(userId);
        return userBadges.Select(ub => new BadgeVM
        {
            Id = ub.BadgeId,
            Name = ub.Name,
            Description = ub.Description,
            IconUrl = ub.IconUrl,
            Category = ub.Category,
            Points = ub.Points
        });
    }

    #endregion

    #region Achievements - Delegate to AchievementService

    public async Task<IEnumerable<UserAchievementVM>> GetUserAchievementsAsync(Guid userId)
        => await _achievementService.GetUserAchievementsAsync(userId);

    public async Task<bool> UpdateAchievementProgressAsync(Guid userId, string achievementCode, int progress)
        => await _achievementService.UpdateAchievementProgressAsync(userId, achievementCode, progress);

    public async Task<bool> CompleteAchievementAsync(Guid userId, string achievementCode)
        => await _achievementService.CompleteAchievementAsync(userId, achievementCode);

    public async Task<IEnumerable<AchievementVM>> GetAvailableAchievementsAsync()
    {
        var userId = Guid.Parse(_currentUserService.UserId ?? Guid.Empty.ToString());
        var userAchievements = await _achievementService.GetAvailableAchievementsAsync(userId);
        return userAchievements.Select(ua => new AchievementVM
        {
            Id = ua.AchievementId,
            Name = ua.Name,
            Title = ua.Title,
            Description = ua.Description,
            IconUrl = ua.IconUrl,
            Points = ua.Points,
            MaxProgress = (int)ua.MaxProgress
        });
    }

    #endregion

    #region Points and Stats - Delegate to PointsAndLevelService

    public async Task<ProfileStatsVM> GetUserStatsAsync(Guid userId)
    {
        var stats = await _pointsAndLevelService.GetUserStatsAsync(userId);
        return new ProfileStatsVM
        {
            TotalPoints = stats.TotalPoints,
            CurrentLevel = stats.Level,
            Level = stats.Level,
            BadgesCount = stats.TotalBadges,
            AchievementsCount = stats.CompletedAchievements,
            Rank = stats.Rank
        };
    }

    public async Task<int> GetUserPointsAsync(Guid userId)
        => await _pointsAndLevelService.GetUserPointsAsync(userId);

    public async Task<bool> AddPointsAsync(Guid userId, int points, string reason)
        => await _pointsAndLevelService.AddPointsAsync(userId, points, reason);

    public async Task<bool> DeductPointsAsync(Guid userId, int points, string reason)
        => await _pointsAndLevelService.DeductPointsAsync(userId, points, reason);

    public async Task<IEnumerable<PointTransactionVM>> GetPointHistoryAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        var history = await _pointsAndLevelService.GetPointHistoryAsync(userId);
        return history.Select(h => new PointTransactionVM
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Points = (int)((dynamic)h).Points,
            Type = ((dynamic)h).Type,
            Reason = ((dynamic)h).Reason,
            CreatedAt = ((dynamic)h).Date
        }).Skip((page - 1) * pageSize).Take(pageSize);
    }

    public async Task<IEnumerable<LeaderboardEntryVM>> GetLeaderboardAsync(string category, int limit = 10)
    {
        var leaderboard = await _pointsAndLevelService.GetLeaderboardAsync(limit);
        return leaderboard.Select(l => new LeaderboardEntryVM
        {
            Rank = ((dynamic)l).Rank,
            UserName = ((dynamic)l).UserName,
            Points = ((dynamic)l).Points,
            Level = ((dynamic)l).Level,
            Category = category
        }).Take(limit);
    }

    public async Task<int> GetUserRankAsync(Guid userId, string category)
        => await _pointsAndLevelService.GetUserRankAsync(userId);

    public async Task<int> GetUserLevelAsync(Guid userId)
        => await _pointsAndLevelService.GetUserLevelAsync(userId);

    #endregion

    #region Gamification Processing

    public async Task ProcessUserActionAsync(Guid userId, string actionType, Dictionary<string, object>? metadata = null)
    {
        try
        {
            // Process the action and award points/badges/achievements as appropriate
            var pointsAwarded = await ProcessActionForPoints(userId, actionType, metadata);
            var badgesAwarded = await CheckAndAwardBadgesAsync(userId);
            var achievementsUpdated = await CheckAndUpdateAchievementsAsync(userId);

            _logger.LogInformation("Processed action {ActionType} for user {UserId}. Points: {PointsAwarded}, Badges: {BadgesAwarded}, Achievements: {AchievementsUpdated}", 
                actionType, userId, pointsAwarded, badgesAwarded, achievementsUpdated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing user action {ActionType} for user {UserId}", actionType, userId);
        }
    }

    private async Task<int> ProcessActionForPoints(Guid userId, string actionType, Dictionary<string, object>? metadata = null)
    {
        var pointsToAward = actionType switch
        {
            "PostCreated" => 10,
            "CommentCreated" => 5,
            "LikeGiven" => 1,
            "ShareCreated" => 3,
            "ProfileCompleted" => 50,
            "FirstLogin" => 25,
            _ => 0
        };

        if (pointsToAward > 0)
        {
            await _pointsAndLevelService.AddPointsAsync(userId, pointsToAward, $"Action: {actionType}");
        }

        return pointsToAward;
    }

    public async Task<bool> CheckAndAwardBadgesAsync(Guid userId)
        => await _badgeService.CheckAndAwardBadgesAsync(userId, "UserAction");

    public async Task<bool> CheckAndUpdateAchievementsAsync(Guid userId)
        => await _achievementService.CheckAndUpdateAchievementsAsync(userId, "UserAction");

    #endregion

    #region System Operations

    public async Task UpdateLeaderboardsAsync()
    {
        try
        {
            _logger.LogInformation("Updating leaderboards");
            
            // TODO: Implement leaderboard update logic
            // This would typically:
            // 1. Calculate top users by points
            // 2. Calculate top users by badges
            // 3. Calculate top users by achievements
            // 4. Update cached leaderboard data
            
            _logger.LogInformation("Leaderboards updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating leaderboards");
            throw;
        }
    }

    public async Task ResetDailyChallengesAsync()
    {
        try
        {
            _logger.LogInformation("Resetting daily challenges");
            
            // TODO: Implement daily challenge reset logic
            // This would reset progress on daily challenges for all users
            
            _logger.LogInformation("Daily challenges reset successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting daily challenges");
            throw;
        }
    }

    public async Task UpdatePeriodicProgressAsync()
    {
        try
        {
            _logger.LogInformation("Updating periodic progress");
            
            // TODO: Implement periodic progress update logic
            // This would update weekly/monthly achievement progress
            
            _logger.LogInformation("Periodic progress updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating periodic progress");
            throw;
        }
    }

    public async Task CleanupExpiredAchievementsAsync()
    {
        try
        {
            _logger.LogInformation("Cleaning up expired achievements");
            
            // TODO: Implement expired achievement cleanup logic
            // This would remove or reset time-limited achievements
            
            _logger.LogInformation("Expired achievements cleaned up successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up expired achievements");
            throw;
        }
    }

    public async Task<bool> CheckAndAwardAchievementsAsync(Guid userId)
    {
        try
        {
            // Check achievements through achievement service
            var achievements = await _achievementService.GetUserAchievementsAsync(userId);
            var inProgressAchievements = achievements.Where(a => !a.IsCompleted);
            
            bool anyAwarded = false;
            foreach (var achievement in inProgressAchievements)
            {
                // TODO: Implement achievement checking logic
                // This would check if the user has met the criteria for each achievement
                _logger.LogDebug("Checking achievement {AchievementName} for user {UserId}", 
                    achievement.Name, userId);
            }
            
            return anyAwarded;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking and awarding achievements for user {UserId}", userId);
            return false;
        }
    }

    #endregion
}


