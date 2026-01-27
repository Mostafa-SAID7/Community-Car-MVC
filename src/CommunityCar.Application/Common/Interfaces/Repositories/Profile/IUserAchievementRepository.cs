using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Profile;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Profile;

public interface IUserAchievementRepository : IBaseRepository<UserAchievement>
{
    Task<IEnumerable<UserAchievement>> GetUserAchievementsAsync(Guid userId, bool completedOnly = false);
    Task<UserAchievement?> GetUserAchievementAsync(Guid userId, string achievementId);
    Task<bool> HasAchievementAsync(Guid userId, string achievementId);
    Task<int> GetCompletedAchievementCountAsync(Guid userId);
    Task<int> GetTotalAchievementCountAsync();
    Task<IEnumerable<UserAchievement>> GetInProgressAchievementsAsync(Guid userId);
    Task<IEnumerable<UserAchievement>> GetRecentlyCompletedAsync(Guid userId, int count = 5);
}


