using CommunityCar.Application.Common.Interfaces.Repositories.Profile;
using CommunityCar.Domain.Entities.Profile;
using CommunityCar.Domain.Enums.Users;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Profile;

public class UserActivityRepository : BaseRepository<UserActivity>, IUserActivityRepository
{
    public UserActivityRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<UserActivity>> GetUserActivitiesAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        return await DbSet
            .Where(ua => ua.UserId == userId)
            .OrderByDescending(ua => ua.ActivityDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserActivity>> GetUserActivitiesByTypeAsync(Guid userId, ActivityType activityType, int page = 1, int pageSize = 20)
    {
        return await DbSet
            .Where(ua => ua.UserId == userId && ua.ActivityType == activityType)
            .OrderByDescending(ua => ua.ActivityDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetUserActivityCountAsync(Guid userId)
    {
        return await DbSet
            .CountAsync(ua => ua.UserId == userId);
    }

    public async Task<int> GetUserActivityCountByTypeAsync(Guid userId, ActivityType activityType)
    {
        return await DbSet
            .CountAsync(ua => ua.UserId == userId && ua.ActivityType == activityType);
    }

    public async Task<IEnumerable<UserActivity>> GetRecentActivitiesAsync(Guid userId, int count = 10)
    {
        return await DbSet
            .Where(ua => ua.UserId == userId)
            .OrderByDescending(ua => ua.ActivityDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task<UserActivity?> GetLastActivityAsync(Guid userId)
    {
        return await DbSet
            .Where(ua => ua.UserId == userId)
            .OrderByDescending(ua => ua.ActivityDate)
            .FirstOrDefaultAsync();
    }
}
