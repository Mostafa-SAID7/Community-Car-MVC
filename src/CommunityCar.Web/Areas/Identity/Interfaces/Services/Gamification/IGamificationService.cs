using CommunityCar.Application.Features.Account.ViewModels.Gamification;
using CommunityCar.Application.Features.Account.ViewModels.Core;

namespace CommunityCar.Web.Areas.Identity.Interfaces.Services.Gamification;

public interface IGamificationService
{
    // Gamification service interface - placeholder for now
    Task<object> GetGamificationAsync();
    Task<ProfileStatsVM> GetUserStatsAsync(Guid userId);
    Task<IEnumerable<UserBadgeVM>> GetUserBadgesAsync(Guid userId);
    Task<IEnumerable<UserAchievementVM>> GetUserAchievementsAsync(Guid userId);
}

