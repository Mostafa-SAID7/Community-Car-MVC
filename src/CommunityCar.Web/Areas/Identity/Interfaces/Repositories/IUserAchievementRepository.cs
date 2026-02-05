using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Account.Gamification;

namespace CommunityCar.Web.Areas.Identity.Interfaces.Repositories;

/// <summary>
/// Repository interface for UserAchievement entity operations
/// </summary>
public interface IUserAchievementRepository : IBaseRepository<UserAchievement>
{
    #region Achievement Management
    Task<IEnumerable<UserAchievement>> GetUserAchievementsAsync(Guid userId);
    Task<UserAchievement?> GetUserAchievementAsync(Guid userId, Guid achievementId);
    Task<bool> HasAchievementAsync(Guid userId, Guid achievementId);
    Task<bool> GrantAchievementAsync(Guid userId, Guid achievementId, DateTime? unlockedAt = null);
    Task<bool> RevokeAchievementAsync(Guid userId, Guid achievementId);
    #endregion

    #region Achievement Analytics
    Task<int> GetAchievementCountAsync(Guid userId);
    Task<IEnumerable<UserAchievement>> GetRecentAchievementsAsync(Guid userId, int count = 10);
    Task<IEnumerable<UserAchievement>> GetAchievementsByTypeAsync(Guid userId, string achievementType);
    Task<Dictionary<Guid, int>> GetAchievementStatisticsAsync();
    Task<IEnumerable<Guid>> GetTopAchieversAsync(Guid achievementId, int count = 10);
    #endregion

    #region Achievement Progress
    Task<double> GetAchievementProgressAsync(Guid userId, Guid achievementId);
    Task<bool> UpdateAchievementProgressAsync(Guid userId, Guid achievementId, double progress);
    Task<IEnumerable<UserAchievement>> GetInProgressAchievementsAsync(Guid userId);
    Task<IEnumerable<UserAchievement>> GetCompletedAchievementsAsync(Guid userId);
    #endregion
}
