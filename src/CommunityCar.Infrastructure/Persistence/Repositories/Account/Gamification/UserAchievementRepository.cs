using CommunityCar.Application.Common.Interfaces.Repositories.Account;
using CommunityCar.Domain.Entities.Account.Gamification;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Account.Gamification;

/// <summary>
/// Repository implementation for UserAchievement entity operations
/// </summary>
public class UserAchievementRepository : BaseRepository<UserAchievement>, IUserAchievementRepository
{
    public UserAchievementRepository(ApplicationDbContext context) : base(context)
    {
    }

    #region Achievement Management

    public async Task<IEnumerable<UserAchievement>> GetUserAchievementsAsync(Guid userId)
    {
        return await Context.UserAchievements
            .Where(ua => ua.UserId == userId)
            .OrderByDescending(ua => ua.UnlockedAt)
            .ToListAsync();
    }

    public async Task<UserAchievement?> GetUserAchievementAsync(Guid userId, Guid achievementId)
    {
        return await Context.UserAchievements
            .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AchievementId == achievementId);
    }

    public async Task<bool> HasAchievementAsync(Guid userId, Guid achievementId)
    {
        return await Context.UserAchievements
            .AnyAsync(ua => ua.UserId == userId && ua.AchievementId == achievementId && ua.IsUnlocked);
    }

    public async Task<bool> GrantAchievementAsync(Guid userId, Guid achievementId, DateTime? unlockedAt = null)
    {
        var existingAchievement = await GetUserAchievementAsync(userId, achievementId);
        
        if (existingAchievement != null)
        {
            if (!existingAchievement.IsUnlocked)
            {
                existingAchievement.Unlock(unlockedAt ?? DateTime.UtcNow);
                await UpdateAsync(existingAchievement);
            }
            return true;
        }

        var userAchievement = UserAchievement.Create(userId, achievementId, unlockedAt ?? DateTime.UtcNow);
        await AddAsync(userAchievement);
        return true;
    }

    public async Task<bool> RevokeAchievementAsync(Guid userId, Guid achievementId)
    {
        var userAchievement = await GetUserAchievementAsync(userId, achievementId);
        
        if (userAchievement == null) return false;

        await DeleteAsync(userAchievement);
        return true;
    }

    #endregion

    #region Achievement Analytics

    public async Task<int> GetAchievementCountAsync(Guid userId)
    {
        return await Context.UserAchievements
            .Where(ua => ua.UserId == userId && ua.IsUnlocked)
            .CountAsync();
    }

    public async Task<IEnumerable<UserAchievement>> GetRecentAchievementsAsync(Guid userId, int count = 10)
    {
        return await Context.UserAchievements
            .Where(ua => ua.UserId == userId && ua.IsUnlocked)
            .OrderByDescending(ua => ua.UnlockedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserAchievement>> GetAchievementsByTypeAsync(Guid userId, string achievementType)
    {
        return await Context.UserAchievements
            .Where(ua => ua.UserId == userId && ua.AchievementType == achievementType && ua.IsUnlocked)
            .OrderByDescending(ua => ua.UnlockedAt)
            .ToListAsync();
    }

    public async Task<Dictionary<Guid, int>> GetAchievementStatisticsAsync()
    {
        return await Context.UserAchievements
            .Where(ua => ua.IsUnlocked)
            .GroupBy(ua => ua.AchievementId)
            .Select(g => new { AchievementId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.AchievementId, x => x.Count);
    }

    public async Task<IEnumerable<Guid>> GetTopAchieversAsync(Guid achievementId, int count = 10)
    {
        return await Context.UserAchievements
            .Where(ua => ua.AchievementId == achievementId && ua.IsUnlocked)
            .OrderBy(ua => ua.UnlockedAt)
            .Take(count)
            .Select(ua => ua.UserId)
            .ToListAsync();
    }

    #endregion

    #region Achievement Progress

    public async Task<double> GetAchievementProgressAsync(Guid userId, Guid achievementId)
    {
        var userAchievement = await GetUserAchievementAsync(userId, achievementId);
        return userAchievement?.Progress ?? 0.0;
    }

    public async Task<bool> UpdateAchievementProgressAsync(Guid userId, Guid achievementId, double progress)
    {
        var userAchievement = await GetUserAchievementAsync(userId, achievementId);
        
        if (userAchievement == null)
        {
            userAchievement = UserAchievement.CreateInProgress(userId, achievementId, progress);
            await AddAsync(userAchievement);
        }
        else
        {
            userAchievement.UpdateProgress(progress);
            await UpdateAsync(userAchievement);
        }

        return true;
    }

    public async Task<IEnumerable<UserAchievement>> GetInProgressAchievementsAsync(Guid userId)
    {
        return await Context.UserAchievements
            .Where(ua => ua.UserId == userId && !ua.IsUnlocked && ua.Progress > 0)
            .OrderByDescending(ua => ua.Progress)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserAchievement>> GetCompletedAchievementsAsync(Guid userId)
    {
        return await Context.UserAchievements
            .Where(ua => ua.UserId == userId && ua.IsUnlocked)
            .OrderByDescending(ua => ua.UnlockedAt)
            .ToListAsync();
    }

    #endregion
}