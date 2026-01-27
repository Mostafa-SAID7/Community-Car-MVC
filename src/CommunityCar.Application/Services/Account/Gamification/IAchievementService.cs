using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;

namespace CommunityCar.Application.Services.Account.Gamification;

/// <summary>
/// Interface for achievement tracking operations
/// </summary>
public interface IAchievementService
{
    Task<IEnumerable<UserAchievementVM>> GetUserAchievementsAsync(Guid userId);
    Task<IEnumerable<UserAchievementVM>> GetAvailableAchievementsAsync(Guid userId);
    Task<bool> UpdateAchievementProgressAsync(Guid userId, string achievementType, int progress);
    Task<bool> CompleteAchievementAsync(Guid userId, string achievementType);
    Task<bool> CheckAndUpdateAchievementsAsync(Guid userId, string actionType);
}


