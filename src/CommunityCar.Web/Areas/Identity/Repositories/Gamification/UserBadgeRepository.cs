using CommunityCar.Web.Areas.Identity.Interfaces.Repositories;
using CommunityCar.Domain.Entities.Account.Gamification;
using CommunityCar.Domain.Enums.Account;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Web.Areas.Identity.Repositories.Gamification;

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
            .OrderByDescending(ub => ub.EarnedAt)
            .ToListAsync();
    }

    public async Task<UserBadge?> GetUserBadgeAsync(Guid userId, Guid badgeId)
    {
        var badgeIdString = badgeId.ToString();
        return await Context.UserBadges
            .FirstOrDefaultAsync(ub => ub.UserId == userId && ub.BadgeId == badgeIdString);
    }

    public async Task<bool> HasBadgeAsync(Guid userId, Guid badgeId)
    {
        var badgeIdString = badgeId.ToString();
        return await Context.UserBadges
            .AnyAsync(ub => ub.UserId == userId && ub.BadgeId == badgeIdString);
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
            .OrderByDescending(ub => ub.EarnedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserBadge>> GetBadgesByRarityAsync(Guid userId, string rarity)
    {
        if (!Enum.TryParse<BadgeRarity>(rarity, true, out var rarityEnum)) return new List<UserBadge>();
        return await Context.UserBadges
            .Where(ub => ub.UserId == userId && ub.Rarity == rarityEnum)
            .OrderByDescending(ub => ub.EarnedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserBadge>> GetBadgesByCategoryAsync(Guid userId, string category)
    {
        if (!Enum.TryParse<BadgeCategory>(category, true, out var categoryEnum)) return new List<UserBadge>();
        return await Context.UserBadges
            .Where(ub => ub.UserId == userId && ub.Category == categoryEnum)
            .OrderByDescending(ub => ub.EarnedAt)
            .ToListAsync();
    }

    public async Task<Dictionary<Guid, int>> GetBadgeStatisticsAsync()
    {
        var stats = await Context.UserBadges
            .GroupBy(ub => ub.BadgeId)
            .Select(g => new { BadgeId = g.Key, Count = g.Count() })
            .ToListAsync();

        return stats
            .Where(x => Guid.TryParse(x.BadgeId, out _))
            .ToDictionary(x => Guid.Parse(x.BadgeId), x => x.Count);
    }

    public async Task<IEnumerable<Guid>> GetTopBadgeHoldersAsync(Guid badgeId, int count = 10)
    {
        var badgeIdString = badgeId.ToString();
        return await Context.UserBadges
            .Where(ub => ub.BadgeId == badgeIdString)
            .OrderBy(ub => ub.EarnedAt)
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
        var badgeIdStrings = badgeOrders.Keys.Select(k => k.ToString()).ToList();
        var userBadges = await Context.UserBadges
            .Where(ub => ub.UserId == userId && badgeIdStrings.Contains(ub.BadgeId))
            .ToListAsync();

        foreach (var userBadge in userBadges)
        {
            if (Guid.TryParse(userBadge.BadgeId, out var badgeGuid) && badgeOrders.TryGetValue(badgeGuid, out var order))
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

