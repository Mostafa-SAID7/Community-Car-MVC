using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;

namespace CommunityCar.Application.Services.Account.Gamification;

/// <summary>
/// Interface for badge management operations
/// </summary>
public interface IBadgeService
{
    Task<IEnumerable<UserBadgeVM>> GetUserBadgesAsync(Guid userId);
    Task<IEnumerable<UserBadgeVM>> GetUserBadgesAsync(Guid userId, bool displayedOnly = false);
    Task<IEnumerable<UserBadgeVM>> GetAvailableBadgesAsync(Guid userId);
    Task<bool> AwardBadgeAsync(Guid userId, string badgeType, string? reason = null);
    Task<bool> RevokeBadgeAsync(Guid userId, string badgeType);
    Task<bool> UpdateBadgeDisplayAsync(Guid userId, Guid badgeId, bool isDisplayed);
    Task<bool> CheckAndAwardBadgesAsync(Guid userId, string actionType);
}


