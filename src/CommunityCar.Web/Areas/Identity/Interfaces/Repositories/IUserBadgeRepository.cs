using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Account.Gamification;

namespace CommunityCar.Web.Areas.Identity.Interfaces.Repositories;

/// <summary>
/// Repository interface for UserBadge entity operations
/// </summary>
public interface IUserBadgeRepository : IBaseRepository<UserBadge>
{
    #region Badge Management
    Task<IEnumerable<UserBadge>> GetUserBadgesAsync(Guid userId);
    Task<UserBadge?> GetUserBadgeAsync(Guid userId, Guid badgeId);
    Task<bool> HasBadgeAsync(Guid userId, Guid badgeId);
    Task<bool> AwardBadgeAsync(Guid userId, Guid badgeId, DateTime? awardedAt = null);
    Task<bool> RevokeBadgeAsync(Guid userId, Guid badgeId);
    #endregion

    #region Badge Analytics
    Task<int> GetBadgeCountAsync(Guid userId);
    Task<IEnumerable<UserBadge>> GetRecentBadgesAsync(Guid userId, int count = 10);
    Task<IEnumerable<UserBadge>> GetBadgesByRarityAsync(Guid userId, string rarity);
    Task<IEnumerable<UserBadge>> GetBadgesByCategoryAsync(Guid userId, string category);
    Task<Dictionary<Guid, int>> GetBadgeStatisticsAsync();
    Task<IEnumerable<Guid>> GetTopBadgeHoldersAsync(Guid badgeId, int count = 10);
    #endregion

    #region Badge Display
    Task<IEnumerable<UserBadge>> GetDisplayBadgesAsync(Guid userId);
    Task<bool> SetBadgeDisplayStatusAsync(Guid userId, Guid badgeId, bool isDisplayed);
    Task<bool> UpdateBadgeDisplayOrderAsync(Guid userId, Dictionary<Guid, int> badgeOrders);
    #endregion
}
