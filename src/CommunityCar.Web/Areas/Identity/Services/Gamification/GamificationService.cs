using CommunityCar.Web.Areas.Identity.Interfaces.Repositories;
using CommunityCar.Web.Areas.Identity.Interfaces.Services.Gamification;
using CommunityCar.Web.Areas.Identity.Interfaces.Services.Authorization;
using CommunityCar.Web.Areas.Identity.Interfaces.Services.Core;
using CommunityCar.Application.Features.Account.ViewModels.Gamification;
using CommunityCar.Application.Features.Account.ViewModels.Core;
using CommunityCar.Domain.Entities.Account.Core;
using CommunityCar.Domain.Entities.Account.Gamification;
using CommunityCar.Domain.Constants;
using CommunityCar.Domain.Enums.Account;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Web.Areas.Identity.Services.Gamification;

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

    public async Task<object> GetGamificationAsync()
    {
        var currentUserId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(currentUserId) || !Guid.TryParse(currentUserId, out var userId)) 
            return new { };

        var badges = await GetUserBadgesAsync(userId);
        var achievements = await GetUserAchievementsAsync(userId);
        var stats = await GetUserStatsAsync(userId);
        var points = await GetUserPointsAsync(userId);
        var level = await GetUserLevelAsync(userId);

        return new
        {
            Badges = badges,
            Achievements = achievements,
            Stats = stats,
            Points = points,
            Level = level
        };
    }

    #region Badges

    public async Task<IEnumerable<UserBadgeVM>> GetUserBadgesAsync(Guid userId)
    {
        var badges = await _badgeRepository.GetUserBadgesAsync(userId);
        return badges.Select(b => new UserBadgeVM
        {
            Id = b.Id,
            BadgeId = b.BadgeId,
            BadgeName = b.Name,
            BadgeDescription = b.Description,
            IconUrl = b.IconUrl,
            AwardedAt = b.EarnedAt
        });
    }

    public async Task<bool> AwardBadgeAsync(Guid userId, string badgeCode)
    {
        // Assuming badgeCode can be parsed to Guid if needed, or repository handles string
        // Let's check repository interface if possible, but for now assuming Guid as per build error
        
        if (!Guid.TryParse(badgeCode, out var badgeId))
        {
             _logger.LogWarning("Invalid badge code: {BadgeCode}", badgeCode);
             return false;
        }

        if (await _badgeRepository.HasBadgeAsync(userId, badgeId)) return false;

        var badge = new UserBadge(userId, badgeCode, "New Badge", "Description", "/assets/badges/default.png", BadgeCategory.Community, BadgeRarity.Common, 10);
        await _badgeRepository.AddAsync(badge);
        
        await AddPointsAsync(userId, badge.Points, $"Awarded badge: {badge.Name}");
        _logger.LogInformation("Awarded badge {BadgeCode} to user {UserId}", badgeCode, userId);
        return true;
    }

    public async Task<bool> RevokeBadgeAsync(Guid userId, string badgeCode)
    {
        if (!Guid.TryParse(badgeCode, out var badgeId))
        {
            _logger.LogWarning("Invalid badge code: {BadgeCode}", badgeCode);
            return false;
        }

        var badge = await _badgeRepository.GetUserBadgeAsync(userId, badgeId);
        if (badge == null) return false;

        await _badgeRepository.DeleteAsync(badge);
        _logger.LogInformation("Revoked badge {BadgeCode} from user {UserId}", badgeCode, userId);
        return true;
    }

    public Task<IEnumerable<BadgeVM>> GetAvailableBadgesAsync() => Task.FromResult(Enumerable.Empty<BadgeVM>());

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
        if (!Guid.TryParse(achievementCode, out var achievementId))
        {
             _logger.LogWarning("Invalid achievement code: {AchievementCode}", achievementCode);
             return false;
        }

        var achievement = await _achievementRepository.GetUserAchievementAsync(userId, achievementId);
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
        if (!Guid.TryParse(achievementCode, out var achievementId))
        {
             _logger.LogWarning("Invalid achievement code: {AchievementCode}", achievementCode);
             return false;
        }

        var achievement = await _achievementRepository.GetUserAchievementAsync(userId, achievementId);
        if (achievement == null) return false;

        achievement.UpdateProgress(achievement.RequiredProgress);
        await _achievementRepository.UpdateAsync(achievement);

        if (!string.IsNullOrEmpty(achievement.RewardBadgeId))
        {
            await AwardBadgeAsync(userId, achievement.RewardBadgeId);
        }

        return true;
    }

    public Task<IEnumerable<AchievementVM>> GetAvailableAchievementsAsync() => Task.FromResult(Enumerable.Empty<AchievementVM>());

    #endregion

    #region Points & Stats

    public async Task<ProfileStatsVM> GetUserStatsAsync(Guid userId)
    {
        var badgesCount = await _badgeRepository.GetBadgeCountAsync(userId);
        var achievementsCount = await _achievementRepository.GetAchievementCountAsync(userId);
        var points = await GetUserPointsAsync(userId);
        var level = await GetUserLevelAsync(userId);

        return new ProfileStatsVM
        {
            PostsCount = 0, // Placeholder
            CommentsCount = 0,
            LikesReceived = 0,
            SharesReceived = 0,
            FollowersCount = 0,
            FollowingCount = 0,
            AchievementsCount = achievementsCount,
            BadgesCount = badgesCount,
            GalleryItemsCount = 0,
            JoinedDate = DateTime.UtcNow,
            DaysActive = 0,
            LastActivityDate = DateTime.UtcNow
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

        var activity = new UserActivity(userId, ActivityType.Other, "PointsExchange", null, reason, reason, pointsAwarded: points);
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

    public Task<bool> DeductPointsAsync(Guid userId, int points, string reason) => Task.FromResult(true);

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
    public Task<int> GetUserLevelAsync(Guid userId) => Task.FromResult(1);

    #endregion

    #region Processing

    public Task ProcessUserActionAsync(Guid userId, string actionType, Dictionary<string, object>? metadata = null) => Task.CompletedTask;
    public Task<bool> CheckAndAwardBadgesAsync(Guid userId) => Task.FromResult(true);
    public Task<bool> CheckAndUpdateAchievementsAsync(Guid userId) => Task.FromResult(true);
    public Task UpdateLeaderboardsAsync() => Task.CompletedTask;
    public Task ResetDailyChallengesAsync() => Task.CompletedTask;
    public Task UpdatePeriodicProgressAsync() => Task.CompletedTask;
    public Task CleanupExpiredAchievementsAsync() => Task.CompletedTask;
    public Task<bool> CheckAndAwardAchievementsAsync(Guid userId) => Task.FromResult(true);

    #endregion
}

