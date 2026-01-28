using CommunityCar.Application.Common.Interfaces.Repositories.Profile;
using CommunityCar.Domain.Entities.Account;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Profile;

public class UserAchievementRepository : BaseRepository<UserAchievement>, IUserAchievementRepository
{
    public UserAchievementRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<UserAchievement>> GetUserAchievementsAsync(Guid userId, bool completedOnly = false)
    {
        var query = DbSet.Where(ua => ua.UserId == userId);
        
        if (completedOnly)
        {
            query = query.Where(ua => ua.IsCompleted);
        }

        return await query
            .OrderByDescending(ua => ua.CompletedAt ?? ua.CreatedAt)
            .ToListAsync();
    }

    public async Task<UserAchievement?> GetUserAchievementAsync(Guid userId, string achievementId)
    {
        return await DbSet
            .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AchievementId == achievementId);
    }

    public async Task<bool> HasAchievementAsync(Guid userId, string achievementId)
    {
        return await DbSet
            .AnyAsync(ua => ua.UserId == userId && ua.AchievementId == achievementId && ua.IsCompleted);
    }

    public async Task<int> GetCompletedAchievementCountAsync(Guid userId)
    {
        return await DbSet
            .CountAsync(ua => ua.UserId == userId && ua.IsCompleted);
    }

    public async Task<int> GetTotalAchievementCountAsync()
    {
        return await DbSet
            .Select(ua => ua.AchievementId)
            .Distinct()
            .CountAsync();
    }

    public async Task<IEnumerable<UserAchievement>> GetInProgressAchievementsAsync(Guid userId)
    {
        return await DbSet
            .Where(ua => ua.UserId == userId && !ua.IsCompleted && ua.CurrentProgress > 0)
            .OrderByDescending(ua => ua.CurrentProgress / (float)ua.RequiredProgress)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserAchievement>> GetRecentlyCompletedAsync(Guid userId, int count = 5)
    {
        return await DbSet
            .Where(ua => ua.UserId == userId && ua.IsCompleted && ua.CompletedAt.HasValue)
            .OrderByDescending(ua => ua.CompletedAt)
            .Take(count)
            .ToListAsync();
    }
}
