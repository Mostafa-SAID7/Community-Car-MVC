using CommunityCar.Application.Features.Profile.DTOs;

namespace CommunityCar.Application.Common.Interfaces.Services.Profile;

public interface IGamificationService
{
    Task<UserGamificationStatsDTO> GetUserStatsAsync(Guid userId);
    Task<List<UserBadgeDTO>> GetUserBadgesAsync(Guid userId, bool displayedOnly = false);
    Task<List<UserAchievementDTO>> GetUserAchievementsAsync(Guid userId, bool completedOnly = false);
    Task<bool> AwardBadgeAsync(Guid userId, string badgeId);
    Task<bool> UpdateAchievementProgressAsync(Guid userId, string achievementId, int progress);
    Task<bool> ToggleBadgeDisplayAsync(Guid badgeId);
    Task InitializeUserGamificationAsync(Guid userId);
    Task ProcessUserActionAsync(Guid userId, string actionType, Dictionary<string, object>? metadata = null);
}