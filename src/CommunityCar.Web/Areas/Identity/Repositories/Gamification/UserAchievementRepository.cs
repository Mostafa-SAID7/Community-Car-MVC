using CommunityCar.Web.Areas.Identity.Interfaces.Repositories;
using CommunityCar.Domain.Entities.Account.Gamification;
using CommunityCar.Domain.Enums.Account;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Web.Areas.Identity.Repositories.Gamification;

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
            .OrderByDescending(ua => ua.CompletedAt)
            .ToListAsync();
    }

    public async Task<UserAchievement?> GetUserAchievementAsync(Guid userId, Guid achievementId)
    {
        var achievementIdString = achievementId.ToString();
        return await Context.UserAchievements
            .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AchievementId == achievementIdString);
    }

    public async Task<bool> HasAchievementAsync(Guid userId, Guid achievementId)
    {
        var achievementIdString = achievementId.ToString();
        return await Context.UserAchievements
            .AnyAsync(ua => ua.UserId == userId && ua.AchievementId == achievementIdString && ua.IsCompleted);
    }

    public async Task<bool> GrantAchievementAsync(Guid userId, Guid achievementId, DateTime? unlockedAt = null)
    {
        var existingAchievement = await GetUserAchievementAsync(userId, achievementId);
        
        if (existingAchievement != null)
        {
            if (!existingAchievement.IsCompleted)
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
            .Where(ua => ua.UserId == userId && ua.IsCompleted)
            .CountAsync();
    }

    public async Task<IEnumerable<UserAchievement>> GetRecentAchievementsAsync(Guid userId, int count = 10)
    {
        return await Context.UserAchievements
            .Where(ua => ua.UserId == userId && ua.IsCompleted)
            .OrderByDescending(ua => ua.CompletedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserAchievement>> GetAchievementsByTypeAsync(Guid userId, string achievementType)
    {
        return await Context.UserAchievements
            .Where(ua => ua.UserId == userId && ua.AchievementType == achievementType && ua.IsCompleted)
            .OrderByDescending(ua => ua.CompletedAt)
            .ToListAsync();
    }

    public async Task<Dictionary<Guid, int>> GetAchievementStatisticsAsync()
    {
        var stats = await Context.UserAchievements
            .Where(ua => ua.IsCompleted)
            .GroupBy(ua => ua.AchievementId)
            .Select(g => new { AchievementId = g.Key, Count = g.Count() })
            .ToListAsync();

        return stats
            .Where(x => Guid.TryParse(x.AchievementId, out _))
            .ToDictionary(x => Guid.Parse(x.AchievementId), x => x.Count);
    }

    public async Task<IEnumerable<Guid>> GetTopAchieversAsync(Guid achievementId, int count = 10)
    {
        var achievementIdString = achievementId.ToString();
        return await Context.UserAchievements
            .Where(ua => ua.AchievementId == achievementIdString && ua.IsCompleted)
            .OrderBy(ua => ua.CompletedAt)
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
            .Where(ua => ua.UserId == userId && !ua.IsCompleted && ua.CurrentProgress > 0)
            .OrderByDescending(ua => ua.CurrentProgress)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserAchievement>> GetCompletedAchievementsAsync(Guid userId)
    {
        return await Context.UserAchievements
            .Where(ua => ua.UserId == userId && ua.IsCompleted)
            .OrderByDescending(ua => ua.CompletedAt)
            .ToListAsync();
    }

    #endregion
}

