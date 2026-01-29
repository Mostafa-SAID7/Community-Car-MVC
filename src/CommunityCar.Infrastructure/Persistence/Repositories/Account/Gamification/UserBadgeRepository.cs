using CommunityCar.Application.Common.Interfaces.Repositories.Account;
using CommunityCar.Domain.Entities.Account.Gamification;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Account.Gamification;

/// <summary>
/// Repository implementation for UserBadge entity operations
/// </summary>
public class UserBadgeRepository : BaseRepository<UserBadge>, IUserBadgeRepository
{
    public UserBadgeRepository(ApplicationDbContext context) : base(context)
    {
    }

    #region Badge Management

    public async Task<IEnumerable<UserBadge>> GetUserBadgesAsync(Guid userId)
    {
        return await Context.UserBadges
            .Where(ub => ub.UserId == userId)
            .OrderByDescending(ub => ub.AwardedAt)
            .ToListAsync();
    }

    public async Task<UserBadge?> GetUserBadgeAsync(Guid userId, Guid badgeId)
    {
        return await Context.UserBadges
            .FirstOrDefaultAsync(ub => ub.UserId == userId && ub.BadgeId == badgeId);
    }

    public async Task<bool> HasBadgeAsync(Guid userId, Guid badgeId)
    {
        return await Context.UserBadges
            .AnyAsync(ub => ub.UserId == userId && ub.BadgeId == badgeId);
    }

    public async Task<bool> AwardBadgeAsync(Guid userId, Guid badgeId, DateTime? awardedAt = null)
    {
        var existingBadge = await GetUserBadgeAsync(userId, badgeId);
        
        if (existingBadge != null) return false; // Badge already awarded

        var userBadge = UserBadge.Create(userId, badgeId, awardedAt ?? DateTime.UtcNow);
        await AddAsync(userBadge);
        return true;
    }

    public async Task<bool> RevokeBadgeAsync(Guid userId, Guid badgeId)
    {
        var userBadge = await GetUserBadgeAsync(userId, badgeId);
        
        if (userBadge == null) return false;

        await DeleteAsync(userBadge);
        return true;
    }

    #endregion

    #region Badge Analytics

    public async Task<int> GetBadgeCountAsync(Guid userId)
    {
        return await Context.UserBadges
            .Where(ub => ub.UserId == userId)
            .CountAsync();
    }

    public async Task<IEnumerable<UserBadge>> GetRecentBadgesAsync(Guid userId, int count = 10)
    {
        return await Context.UserBadges
            .Where(ub => ub.UserId == userId)
            .OrderByDescending(ub => ub.AwardedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserBadge>> GetBadgesByRarityAsync(Guid userId, string rarity)
    {
        return await Context.UserBadges
            .Where(ub => ub.UserId == userId && ub.Rarity == rarity)
            .OrderByDescending(ub => ub.AwardedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserBadge>> GetBadgesByCategoryAsync(Guid userId, string category)
    {
        return await Context.UserBadges
            .Where(ub => ub.UserId == userId && ub.Category == category)
            .OrderByDescending(ub => ub.AwardedAt)
            .ToListAsync();
    }

    public async Task<Dictionary<Guid, int>> GetBadgeStatisticsAsync()
    {
        return await Context.UserBadges
            .GroupBy(ub => ub.BadgeId)
            .Select(g => new { BadgeId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.BadgeId, x => x.Count);
    }

    public async Task<IEnumerable<Guid>> GetTopBadgeHoldersAsync(Guid badgeId, int count = 10)
    {
        return await Context.UserBadges
            .Where(ub => ub.BadgeId == badgeId)
            .OrderBy(ub => ub.AwardedAt)
            .Take(count)
            .Select(ub => ub.UserId)
            .ToListAsync();
    }

    #endregion

    #region Badge Display

    public async Task<IEnumerable<UserBadge>> GetDisplayBadgesAsync(Guid userId)
    {
        return await Context.UserBadges
            .Where(ub => ub.UserId == userId && ub.IsDisplayed)
            .OrderBy(ub => ub.DisplayOrder)
            .ToListAsync();
    }

    public async Task<bool> SetBadgeDisplayStatusAsync(Guid userId, Guid badgeId, bool isDisplayed)
    {
        var userBadge = await GetUserBadgeAsync(userId, badgeId);
        
        if (userBadge == null) return false;

        userBadge.SetDisplayStatus(isDisplayed);
        await UpdateAsync(userBadge);
        return true;
    }

    public async Task<bool> UpdateBadgeDisplayOrderAsync(Guid userId, Dictionary<Guid, int> badgeOrders)
    {
        var userBadges = await Context.UserBadges
            .Where(ub => ub.UserId == userId && badgeOrders.Keys.Contains(ub.BadgeId))
            .ToListAsync();

        foreach (var userBadge in userBadges)
        {
            if (badgeOrders.TryGetValue(userBadge.BadgeId, out var order))
            {
                userBadge.SetDisplayOrder(order);
            }
        }

        if (userBadges.Any())
        {
            Context.UserBadges.UpdateRange(userBadges);
            return true;
        }

        return false;
    }

    #endregion
}