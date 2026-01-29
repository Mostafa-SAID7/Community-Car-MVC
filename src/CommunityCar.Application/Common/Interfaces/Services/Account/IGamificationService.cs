using CommunityCar.Application.Features.Account.ViewModels.Gamification;
using CommunityCar.Application.Features.Account.ViewModels.Core;

namespace CommunityCar.Application.Common.Interfaces.Services.Account;

/// <summary>
/// Interface for gamification features (badges, achievements, points)
/// </summary>
public interface IGamificationService
{
    // Badges
    Task<IEnumerable<UserBadgeVM>> GetUserBadgesAsync(Guid userId);
    Task<bool> AwardBadgeAsync(Guid userId, string badgeCode);
    Task<bool> RevokeBadgeAsync(Guid userId, string badgeCode);
    Task<IEnumerable<BadgeVM>> GetAvailableBadgesAsync();

    // Achievements
    Task<IEnumerable<UserAchievementVM>> GetUserAchievementsAsync(Guid userId);
    Task<bool> UpdateAchievementProgressAsync(Guid userId, string achievementCode, int progress);
    Task<bool> CompleteAchievementAsync(Guid userId, string achievementCode);
    Task<IEnumerable<AchievementVM>> GetAvailableAchievementsAsync();

    // Points System
    Task<int> GetUserPointsAsync(Guid userId);
    Task<bool> AddPointsAsync(Guid userId, int points, string reason);
    Task<bool> DeductPointsAsync(Guid userId, int points, string reason);
    Task<IEnumerable<PointTransactionVM>> GetPointHistoryAsync(Guid userId, int page = 1, int pageSize = 20);

    // Leaderboards
    Task<IEnumerable<LeaderboardEntryVM>> GetLeaderboardAsync(string category, int limit = 10);
    Task<int> GetUserRankAsync(Guid userId, string category);
    Task<int> GetUserLevelAsync(Guid userId);

    // Gamification Events
    Task ProcessUserActionAsync(Guid userId, string action, Dictionary<string, object>? metadata = null);
    Task<bool> CheckAndAwardBadgesAsync(Guid userId);
    Task<bool> CheckAndUpdateAchievementsAsync(Guid userId);
    Task<bool> CheckAndAwardAchievementsAsync(Guid userId);

    // System Operations
    Task UpdateLeaderboardsAsync();
    Task ResetDailyChallengesAsync();
    Task UpdatePeriodicProgressAsync();
    Task CleanupExpiredAchievementsAsync();

    // User Statistics
    Task<ProfileStatsVM> GetUserStatsAsync(Guid userId);
}
