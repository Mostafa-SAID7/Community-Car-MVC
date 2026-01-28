using CommunityCar.Application.Common.Interfaces.Repositories.Profile;
using CommunityCar.Domain.Entities.Account.Gamification;
using CommunityCar.Domain.Enums.Account;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Profile;

public class UserBadgeRepository : BaseRepository<UserBadge>, IUserBadgeRepository
{
    public UserBadgeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<UserBadge>> GetUserBadgesAsync(Guid userId, bool displayedOnly = false)
    {
        var query = DbSet.Where(ub => ub.UserId == userId);
        
        if (displayedOnly)
        {
            query = query.Where(ub => ub.IsDisplayed);
        }

        return await query
            .OrderByDescending(ub => ub.EarnedAt)
            .ToListAsync();
    }

    public async Task<UserBadge?> GetUserBadgeAsync(Guid userId, string badgeId)
    {
        return await DbSet
            .FirstOrDefaultAsync(ub => ub.UserId == userId && ub.BadgeId == badgeId);
    }

    public async Task<bool> HasBadgeAsync(Guid userId, string badgeId)
    {
        return await DbSet
            .AnyAsync(ub => ub.UserId == userId && ub.BadgeId == badgeId);
    }

    public async Task<int> GetUserBadgeCountAsync(Guid userId)
    {
        return await DbSet
            .CountAsync(ub => ub.UserId == userId);
    }

    public async Task<IEnumerable<UserBadge>> GetRecentBadgesAsync(Guid userId, int count = 5)
    {
        return await DbSet
            .Where(ub => ub.UserId == userId)
            .OrderByDescending(ub => ub.EarnedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserBadge>> GetBadgesByCategoryAsync(Guid userId, BadgeCategory category)
    {
        return await DbSet
            .Where(ub => ub.UserId == userId && ub.Category == category)
            .OrderByDescending(ub => ub.EarnedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserBadge>> GetBadgesByRarityAsync(Guid userId, BadgeRarity rarity)
    {
        return await DbSet
            .Where(ub => ub.UserId == userId && ub.Rarity == rarity)
            .OrderByDescending(ub => ub.EarnedAt)
            .ToListAsync();
    }
}
