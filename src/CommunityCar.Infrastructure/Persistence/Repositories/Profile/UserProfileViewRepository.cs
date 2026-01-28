using CommunityCar.Application.Common.Interfaces.Repositories.Profile;
using CommunityCar.Domain.Entities.Account.Profile;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Profile;

public class UserProfileViewRepository : BaseRepository<UserProfileView>, IUserProfileViewRepository
{
    public UserProfileViewRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<UserProfileView>> GetProfileViewsAsync(Guid profileUserId, int page = 1, int pageSize = 20)
    {
        return await Context.Set<UserProfileView>()
            .Where(v => v.ProfileUserId == profileUserId)
            .OrderByDescending(v => v.ViewedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserProfileView>> GetUserViewHistoryAsync(Guid viewerId, int page = 1, int pageSize = 20)
    {
        return await Context.Set<UserProfileView>()
            .Where(v => v.ViewerId == viewerId && !v.IsAnonymous)
            .OrderByDescending(v => v.ViewedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetProfileViewCountAsync(Guid profileUserId, DateTime? since = null)
    {
        var query = Context.Set<UserProfileView>()
            .Where(v => v.ProfileUserId == profileUserId);

        if (since.HasValue)
            query = query.Where(v => v.ViewedAt >= since.Value);

        return await query.CountAsync();
    }

    public async Task<int> GetUniqueViewersCountAsync(Guid profileUserId, DateTime? since = null)
    {
        var query = Context.Set<UserProfileView>()
            .Where(v => v.ProfileUserId == profileUserId && !v.IsAnonymous);

        if (since.HasValue)
            query = query.Where(v => v.ViewedAt >= since.Value);

        return await query
            .Select(v => v.ViewerId)
            .Distinct()
            .CountAsync();
    }

    public async Task<UserProfileView?> GetRecentViewAsync(Guid viewerId, Guid profileUserId, int minutesThreshold = 30)
    {
        var cutoffTime = DateTime.UtcNow.AddMinutes(-minutesThreshold);
        
        return await Context.Set<UserProfileView>()
            .Where(v => v.ViewerId == viewerId && v.ProfileUserId == profileUserId && v.ViewedAt >= cutoffTime)
            .OrderByDescending(v => v.ViewedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<UserProfileView>> GetViewsBySourceAsync(Guid profileUserId, string viewSource, int page = 1, int pageSize = 20)
    {
        return await Context.Set<UserProfileView>()
            .Where(v => v.ProfileUserId == profileUserId && v.ViewSource == viewSource)
            .OrderByDescending(v => v.ViewedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Dictionary<string, int>> GetViewSourceStatsAsync(Guid profileUserId, DateTime? since = null)
    {
        var query = Context.Set<UserProfileView>()
            .Where(v => v.ProfileUserId == profileUserId);

        if (since.HasValue)
            query = query.Where(v => v.ViewedAt >= since.Value);

        return await query
            .GroupBy(v => v.ViewSource ?? "unknown")
            .Select(g => new { Source = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Source, x => x.Count);
    }

    public async Task<IEnumerable<UserProfileView>> GetTopViewersAsync(Guid profileUserId, int limit = 10, DateTime? since = null)
    {
        var query = Context.Set<UserProfileView>()
            .Where(v => v.ProfileUserId == profileUserId && !v.IsAnonymous);

        if (since.HasValue)
            query = query.Where(v => v.ViewedAt >= since.Value);

        return await query
            .GroupBy(v => v.ViewerId)
            .Select(g => new { ViewerId = g.Key, Count = g.Count(), LastView = g.Max(v => v.ViewedAt) })
            .OrderByDescending(x => x.Count)
            .ThenByDescending(x => x.LastView)
            .Take(limit)
            .SelectMany(x => Context.Set<UserProfileView>()
                .Where(v => v.ViewerId == x.ViewerId && v.ProfileUserId == profileUserId)
                .OrderByDescending(v => v.ViewedAt)
                .Take(1))
            .ToListAsync();
    }

    public async Task<bool> HasViewedProfileAsync(Guid viewerId, Guid profileUserId)
    {
        return await Context.Set<UserProfileView>()
            .AnyAsync(v => v.ViewerId == viewerId && v.ProfileUserId == profileUserId);
    }

    public async Task<DateTime?> GetLastViewDateAsync(Guid viewerId, Guid profileUserId)
    {
        return await Context.Set<UserProfileView>()
            .Where(v => v.ViewerId == viewerId && v.ProfileUserId == profileUserId)
            .OrderByDescending(v => v.ViewedAt)
            .Select(v => v.ViewedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<UserProfileView>> GetMutualViewsAsync(Guid userId1, Guid userId2)
    {
        return await Context.Set<UserProfileView>()
            .Where(v => (v.ViewerId == userId1 && v.ProfileUserId == userId2) ||
                       (v.ViewerId == userId2 && v.ProfileUserId == userId1))
            .OrderByDescending(v => v.ViewedAt)
            .ToListAsync();
    }

    public async Task<int> GetDailyViewCountAsync(Guid profileUserId, DateTime date)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        return await Context.Set<UserProfileView>()
            .Where(v => v.ProfileUserId == profileUserId && 
                       v.ViewedAt >= startOfDay && 
                       v.ViewedAt < endOfDay)
            .CountAsync();
    }

    public async Task<Dictionary<DateTime, int>> GetViewTrendsAsync(Guid profileUserId, DateTime startDate, DateTime endDate)
    {
        return await Context.Set<UserProfileView>()
            .Where(v => v.ProfileUserId == profileUserId && 
                       v.ViewedAt >= startDate && 
                       v.ViewedAt <= endDate)
            .GroupBy(v => v.ViewedAt.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Date, x => x.Count);
    }

    public async Task CleanupOldViewsAsync(DateTime cutoffDate)
    {
        var oldViews = await Context.Set<UserProfileView>()
            .Where(v => v.ViewedAt < cutoffDate)
            .ToListAsync();

        Context.Set<UserProfileView>().RemoveRange(oldViews);
        await Context.SaveChangesAsync();
    }
}