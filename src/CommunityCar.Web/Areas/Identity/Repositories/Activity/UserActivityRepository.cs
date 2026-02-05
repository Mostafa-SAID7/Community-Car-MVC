using CommunityCar.Web.Areas.Identity.Interfaces.Repositories;
using CommunityCar.Domain.Entities.Account.Core;
using CommunityCar.Domain.Enums.Account;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Web.Areas.Identity.Repositories.Activity;

/// <summary>
/// Repository implementation for UserActivity entity operations
/// </summary>
public class UserActivityRepository : BaseRepository<UserActivity>, IUserActivityRepository
{
    public UserActivityRepository(ApplicationDbContext context) : base(context)
    {
    }

    #region Activity Tracking

    public async Task<IEnumerable<UserActivity>> GetUserActivitiesAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        return await Context.UserActivities
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserActivity>> GetRecentActivitiesAsync(Guid userId, int count = 10)
    {
        return await Context.UserActivities
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserActivity>> GetActivitiesByTypeAsync(Guid userId, string activityType)
    {
        if (!Enum.TryParse<ActivityType>(activityType, true, out var typeEnum))
            return new List<UserActivity>();

        return await Context.UserActivities
            .Where(a => a.UserId == userId && a.ActivityType == typeEnum)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserActivity>> GetActivitiesInDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate)
    {
        return await Context.UserActivities
            .Where(a => a.UserId == userId && a.CreatedAt >= startDate && a.CreatedAt <= endDate)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    #endregion

    #region Activity Analytics

    public async Task<int> GetActivityCountAsync(Guid userId, DateTime? fromDate = null)
    {
        var query = Context.UserActivities.Where(a => a.UserId == userId);
        
        if (fromDate.HasValue)
        {
            query = query.Where(a => a.CreatedAt >= fromDate.Value);
        }

        return await query.CountAsync();
    }

    public async Task<Dictionary<string, int>> GetActivityCountByTypeAsync(Guid userId, DateTime? fromDate = null)
    {
        var query = Context.UserActivities.Where(a => a.UserId == userId);
        
        if (fromDate.HasValue)
        {
            query = query.Where(a => a.CreatedAt >= fromDate.Value);
        }

        var stats = await query
            .GroupBy(a => a.ActivityType)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToListAsync();

        return stats.ToDictionary(x => x.Type.ToString(), x => x.Count);
    }

    public async Task<IEnumerable<UserActivity>> GetMostActiveUsersAsync(DateTime fromDate, int count = 10)
    {
        return await Context.UserActivities
            .Where(a => a.CreatedAt >= fromDate)
            .GroupBy(a => a.UserId)
            .OrderByDescending(g => g.Count())
            .Take(count)
            .SelectMany(g => g.Take(1))
            .ToListAsync();
    }

    public async Task<DateTime?> GetLastActivityDateAsync(Guid userId)
    {
        var lastActivity = await Context.UserActivities
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .FirstOrDefaultAsync();

        return lastActivity?.CreatedAt;
    }

    #endregion

    #region Activity Management

    public async Task LogActivityAsync(Guid userId, string activityType, string description, Dictionary<string, object>? metadata = null)
    {
        if (!Enum.TryParse<ActivityType>(activityType, true, out var typeEnum))
            typeEnum = ActivityType.Other;

        var activity = UserActivity.Create(userId, typeEnum, "General", null, description);
        if (metadata != null)
        {
            // Assuming Metadata property exists and takes a string or can be handled
            // For now, if entity has SetMetadata(string)
            // activity.SetMetadata(JsonSerializer.Serialize(metadata)); 
        }
        await AddAsync(activity);
    }

    public async Task<bool> DeleteOldActivitiesAsync(DateTime cutoffDate)
    {
        var oldActivities = await Context.UserActivities
            .Where(a => a.CreatedAt < cutoffDate)
            .ToListAsync();

        if (oldActivities.Any())
        {
            Context.UserActivities.RemoveRange(oldActivities);
            return true;
        }

        return false;
    }

    public async Task<bool> DeleteUserActivitiesAsync(Guid userId)
    {
        var userActivities = await Context.UserActivities
            .Where(a => a.UserId == userId)
            .ToListAsync();

        if (userActivities.Any())
        {
            Context.UserActivities.RemoveRange(userActivities);
            return true;
        }

        return false;
    }

    #endregion
}

