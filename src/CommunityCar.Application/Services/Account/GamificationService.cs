using CommunityCar.Application.Common.Interfaces.Repositories.Profile;
using CommunityCar.Application.Common.Interfaces.Repositories.User;
using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Common.Interfaces.Services.Authorization;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Domain.Entities.Account.Core;
using CommunityCar.Domain.Entities.Account.Gamification;
using CommunityCar.Domain.Constants;
using CommunityCar.Domain.Enums.Account;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace CommunityCar.Application.Services.Account;

public class GamificationService : IGamificationService
{
    private readonly IUserBadgeRepository _badgeRepository;
    private readonly IUserAchievementRepository _achievementRepository;
    private readonly IUserActivityRepository _activityRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleService _roleService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<GamificationService> _logger;

    public GamificationService(
        IUserBadgeRepository badgeRepository,
        IUserAchievementRepository achievementRepository,
        IUserActivityRepository activityRepository,
        IUserRepository userRepository,
        IRoleService roleService,
        ICurrentUserService currentUserService,
        ILogger<GamificationService> logger)
    {
        _badgeRepository = badgeRepository;
        _achievementRepository = achievementRepository;
        _activityRepository = activityRepository;
        _userRepository = userRepository;
        _roleService = roleService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    #region Badges

    public async Task<IEnumerable<UserBadgeVM>> GetUserBadgesAsync(Guid userId)
    {
        var badges = await _badgeRepository.GetUserBadgesAsync(userId);
        return badges.Select(b => new UserBadgeVM
        {
            Id = b.Id,
            BadgeId = b.BadgeId,
            Name = b.Name,
            Description = b.Description,
            IconUrl = b.IconUrl,
            AwardedAt = b.EarnedAt
        });
    }

    public async Task<bool> AwardBadgeAsync(Guid userId, string badgeCode)
    {
        if (await _badgeRepository.HasBadgeAsync(userId, badgeCode)) return false;

        // In a real app, you'd fetch the badge template/details by badgeCode
        var badge = new UserBadge(userId, badgeCode, "New Badge", "Description", "/assets/badges/default.png", BadgeCategory.Community, BadgeRarity.Common, 10);
        await _badgeRepository.AddAsync(badge);
        
        await AddPointsAsync(userId, badge.Points, $"Awarded badge: {badge.Name}");
        _logger.LogInformation("Awarded badge {BadgeCode} to user {UserId}", badgeCode, userId);
        return true;
    }

    public async Task<bool> RevokeBadgeAsync(Guid userId, string badgeCode)
    {
        var badge = await _badgeRepository.GetUserBadgeAsync(userId, badgeCode);
        if (badge == null) return false;

        await _badgeRepository.DeleteAsync(badge);
        _logger.LogInformation("Revoked badge {BadgeCode} from user {UserId}", badgeCode, userId);
        return true;
    }

    public async Task<IEnumerable<BadgeVM>> GetAvailableBadgesAsync()
    {
        // Placeholder for fetching all available badge definitions
        return new List<BadgeVM>();
    }

    #endregion

    #region Achievements

    public async Task<IEnumerable<UserAchievementVM>> GetUserAchievementsAsync(Guid userId)
    {
        var achievements = await _achievementRepository.GetUserAchievementsAsync(userId);
        return achievements.Select(a => new UserAchievementVM
        {
            Id = a.Id,
            AchievementId = a.AchievementId,
            Name = a.Title,
            Title = a.Title,
            Description = a.Description,
            IconUrl = "/assets/achievements/default.png",
            IsCompleted = a.IsCompleted,
            CompletedAt = a.CompletedAt,
            Progress = a.CurrentProgress,
            Points = a.RewardPoints
        });
    }

    public async Task<bool> UpdateAchievementProgressAsync(Guid userId, string achievementCode, int progress)
    {
        var achievement = await _achievementRepository.GetUserAchievementAsync(userId, achievementCode);
        if (achievement == null) return false;

        achievement.UpdateProgress(progress);
        await _achievementRepository.UpdateAsync(achievement);

        if (achievement.IsCompleted && !string.IsNullOrEmpty(achievement.RewardBadgeId))
        {
            await AwardBadgeAsync(userId, achievement.RewardBadgeId);
        }

        return true;
    }

    public async Task<bool> CompleteAchievementAsync(Guid userId, string achievementCode)
    {
        var achievement = await _achievementRepository.GetUserAchievementAsync(userId, achievementCode);
        if (achievement == null) return false;

        achievement.UpdateProgress(achievement.RequiredProgress);
        await _achievementRepository.UpdateAsync(achievement);

        if (!string.IsNullOrEmpty(achievement.RewardBadgeId))
        {
            await AwardBadgeAsync(userId, achievement.RewardBadgeId);
        }

        return true;
    }

    public async Task<IEnumerable<AchievementVM>> GetAvailableAchievementsAsync()
    {
        // Placeholder for fetching all available achievement definitions
        return new List<AchievementVM>();
    }

    #endregion

    #region Points & Stats

    public async Task<ProfileStatsVM> GetUserStatsAsync(Guid userId)
    {
        var badgesCount = await _badgeRepository.GetUserBadgeCountAsync(userId);
        var achievementsCount = await _achievementRepository.GetCompletedAchievementCountAsync(userId);
        var points = await GetUserPointsAsync(userId);
        var level = await GetUserLevelAsync(userId);

        return new CommunityCar.Application.Common.Models.Profile.ProfileStatsVM
        {
            TotalPoints = points,
            Level = level,
            BadgesCount = badgesCount,
            AchievementsCount = achievementsCount,
            ExperiencePoints = (points % 1000), // Simple logic
            Rank = await GetUserRankAsync(userId, "points")
        };
    }

    public async Task<int> GetUserPointsAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user?.TotalPoints ?? 0;
    }

    public async Task<bool> AddPointsAsync(Guid userId, int points, string reason)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        user.TotalPoints += points;
        await _userRepository.UpdateAsync(user);

        var activity = new UserActivity(userId, ActivityType.Other, "PointsExchange", null, reason, reason, points);
        await _activityRepository.AddAsync(activity);

        await CheckAndPromoteUserAsync(userId);

        return true;
    }

    private async Task CheckAndPromoteUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return;

        string? targetRole = null;

        if (user.TotalPoints >= Progression.Thresholds.Master)
            targetRole = Roles.Master;
        else if (user.TotalPoints >= Progression.Thresholds.Author)
            targetRole = Roles.Author;
        else if (user.TotalPoints >= Progression.Thresholds.Reviewer)
            targetRole = Roles.Reviewer;
        else if (user.TotalPoints >= Progression.Thresholds.Expert)
            targetRole = Roles.Expert;

        if (targetRole != null)
        {
            var userRoles = await _roleService.GetUserRolesAsync(userId);
            if (!userRoles.Contains(targetRole))
            {
                await _roleService.AssignRoleAsync(userId, targetRole);
                _logger.LogInformation("User {UserId} promoted to {Role}", userId, targetRole);
            }
        }
    }

    public Task<bool> DeductPointsAsync(Guid userId, int points, string reason)
    {
        // Implementation for deducting points if needed (negative points activity)
        return Task.FromResult(true);
    }

    public async Task<IEnumerable<PointTransactionVM>> GetPointHistoryAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        var activities = await _activityRepository.GetUserActivitiesAsync(userId, page, pageSize);
        return activities.Select(a => new PointTransactionVM
        {
            Amount = a.PointsAwarded,
            Reason = a.Description ?? "No reason provided",
            Timestamp = a.ActivityDate
        });
    }

    #endregion

    #region Leaderboard

    public Task<IEnumerable<LeaderboardEntryVM>> GetLeaderboardAsync(string category, int limit = 10) => Task.FromResult(Enumerable.Empty<LeaderboardEntryVM>());
    public Task<int> GetUserRankAsync(Guid userId, string category) => Task.FromResult(1);
    public Task<int> GetUserLevelAsync(Guid userId) 
    {
        // Simple level logic: 1 level per 1000 points
        return Task.FromResult(1); // placeholder for calculation
    }

    #endregion

    #region Processing

    public async Task ProcessUserActionAsync(Guid userId, string actionType, Dictionary<string, object>? metadata = null)
    {
        _logger.LogInformation("Processing action {Action} for user {UserId}", actionType, userId);
        
        // Map actionType to ActivityType and award points
        // This would normally be handled by a more complex rule engine
        await Task.CompletedTask;
    }

    public Task<bool> CheckAndAwardBadgesAsync(Guid userId) => Task.FromResult(true);
    public Task<bool> CheckAndUpdateAchievementsAsync(Guid userId) => Task.FromResult(true);
    public Task UpdateLeaderboardsAsync() => Task.CompletedTask;
    public Task ResetDailyChallengesAsync() => Task.CompletedTask;
    public Task UpdatePeriodicProgressAsync() => Task.CompletedTask;
    public Task CleanupExpiredAchievementsAsync() => Task.CompletedTask;
    public Task<bool> CheckAndAwardAchievementsAsync(Guid userId) => Task.FromResult(true);

    #endregion
}
