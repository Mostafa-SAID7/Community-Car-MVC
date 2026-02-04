using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Features.Account.ViewModels.Gamification;

namespace CommunityCar.Application.Common.Interfaces.Services.Account.Gamification;

public interface IGamificationService
{
    Task<Result<UserGamificationVM>> GetUserGamificationAsync(Guid userId);
    Task<Result<List<AchievementVM>>> GetUserAchievementsAsync(Guid userId);
    Task<Result<List<BadgeVM>>> GetUserBadgesAsync(Guid userId);
    Task<Result<LeaderboardVM>> GetLeaderboardAsync(string category, int page = 1, int pageSize = 20);
    Task<Result> AwardPointsAsync(Guid userId, int points, string reason);
    Task<Result> AwardBadgeAsync(Guid userId, string badgeId, string reason);
    Task<Result> AwardAchievementAsync(Guid userId, string achievementId);
    Task<Result<List<QuestVM>>> GetAvailableQuestsAsync(Guid userId);
    Task<Result<List<QuestVM>>> GetUserQuestsAsync(Guid userId);
    Task<Result> StartQuestAsync(Guid userId, string questId);
    Task<Result> CompleteQuestAsync(Guid userId, string questId);
    Task<Result<GamificationStatsVM>> GetGamificationStatsAsync(Guid userId);
}