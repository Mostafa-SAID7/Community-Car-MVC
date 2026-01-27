using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Profile;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Profile;

public interface IUserBadgeRepository : IBaseRepository<UserBadge>
{
    Task<IEnumerable<UserBadge>> GetUserBadgesAsync(Guid userId, bool displayedOnly = false);
    Task<UserBadge?> GetUserBadgeAsync(Guid userId, string badgeId);
    Task<bool> HasBadgeAsync(Guid userId, string badgeId);
    Task<int> GetUserBadgeCountAsync(Guid userId);
    Task<IEnumerable<UserBadge>> GetRecentBadgesAsync(Guid userId, int count = 5);
    Task<IEnumerable<UserBadge>> GetBadgesByCategoryAsync(Guid userId, BadgeCategory category);
    Task<IEnumerable<UserBadge>> GetBadgesByRarityAsync(Guid userId, BadgeRarity rarity);
}


