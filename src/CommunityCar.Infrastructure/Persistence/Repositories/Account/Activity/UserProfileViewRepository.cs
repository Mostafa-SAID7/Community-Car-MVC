using CommunityCar.Application.Common.Interfaces.Repositories.Account;
using CommunityCar.Domain.Entities.Account.Profile;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Account.Activity;

/// <summary>
/// Repository implementation for UserProfileView entity operations
/// </summary>
public class UserProfileViewRepository : BaseRepository<UserProfileView>, IUserProfileViewRepository
{
    public UserProfileViewRepository(ApplicationDbContext context) : base(context)
    {
    }

    #region Profile View Tracking

    public async Task<bool> RecordProfileViewAsync(Guid viewerId, Guid profileUserId, string? ipAddress = null, string? userAgent = null)
    {
        if (viewerId == profileUserId) return false; // Don't record self-views

        // Check if view already exists within a certain timeframe (e.g., last hour)
        var recentView = await Context.UserProfileViews
            .Where(upv => upv.ViewerId == viewerId && upv.ProfileUserId == profileUserId)
            .Where(upv => upv.ViewedAt >= DateTime.UtcNow.AddHours(-1))
            .FirstOrDefaultAsync();

        if (recentView != null)
        {
            // Update existing view
            recentView.UpdateView(ipAddress, userAgent);
            await UpdateAsync(recentView);
        }
        else
        {
            // Create new view record
            var profileView = UserProfileView.Create(viewerId, profileUserId, ipAddress, userAgent);
            await AddAsync(profileView);
        }

        return true;
    }

    public async Task<IEnumerable<UserProfileView>> GetProfileViewsAsync(Guid profileUserId, int page = 1, int pageSize = 20)
    {
        return await Context.UserProfileViews
            .Where(upv => upv.ProfileUserId == profileUserId)
            .OrderByDescending(upv => upv.ViewedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserProfileView>> GetUserViewHistoryAsync(Guid viewerId, int page = 1, int pageSize = 20)
    {
        return await Context.UserProfileViews
            .Where(upv => upv.ViewerId == viewerId)
            .OrderByDescending(upv => upv.ViewedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<UserProfileView?> GetLastViewAsync(Guid viewerId, Guid profileUserId)
    {
        return await Context.UserProfileViews
            .Where(upv => upv.ViewerId == viewerId && upv.ProfileUserId == profileUserId)
            .OrderByDescending(upv => upv.ViewedAt)
            .FirstOrDefaultAsync();
    }

    #endregion

    #region View Analytics

    public async Task<int> GetProfileViewCountAsync(Guid profileUserId, DateTime? fromDate = null)
    {
        var query = Context.UserProfileViews.Where(upv => upv.ProfileUserId == profileUserId);
        
        if (fromDate.HasValue)
        {
            query = query.Where(upv => upv.ViewedAt >= fromDate.Value);
        }

        return await query.CountAsync();
    }

    public async Task<int> GetUniqueViewerCountAsync(Guid profileUserId, DateTime? fromDate = null)
    {
        var query = Context.UserProfileViews.Where(upv => upv.ProfileUserId == profileUserId);
        
        if (fromDate.HasValue)
        {
            query = query.Where(upv => upv.ViewedAt >= fromDate.Value);
        }

        return await query
            .Select(upv => upv.ViewerId)
            .Distinct()
            .CountAsync();
    }

    public async Task<IEnumerable<UserProfileView>> GetRecentViewsAsync(Guid profileUserId, int count = 10)
    {
        return await Context.UserProfileViews
            .Where(upv => upv.ProfileUserId == profileUserId)
            .OrderByDescending(upv => upv.ViewedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Dictionary<DateTime, int>> GetViewsByDateAsync(Guid profileUserId, DateTime fromDate, DateTime toDate)
    {
        return await Context.UserProfileViews
            .Where(upv => upv.ProfileUserId == profileUserId && upv.ViewedAt >= fromDate && upv.ViewedAt <= toDate)
            .GroupBy(upv => upv.ViewedAt.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Date, x => x.Count);
    }

    public async Task<IEnumerable<Guid>> GetTopViewersAsync(Guid profileUserId, int count = 10)
    {
        return await Context.UserProfileViews
            .Where(upv => upv.ProfileUserId == profileUserId)
            .GroupBy(upv => upv.ViewerId)
            .OrderByDescending(g => g.Count())
            .Take(count)
            .Select(g => g.Key)
            .ToListAsync();
    }

    #endregion

    #region View Statistics

    public async Task<IEnumerable<Guid>> GetMostViewedProfilesAsync(int count = 10, DateTime? fromDate = null)
    {
        var query = Context.UserProfileViews.AsQueryable();
        
        if (fromDate.HasValue)
        {
            query = query.Where(upv => upv.ViewedAt >= fromDate.Value);
        }

        return await query
            .GroupBy(upv => upv.ProfileUserId)
            .OrderByDescending(g => g.Count())
            .Take(count)
            .Select(g => g.Key)
            .ToListAsync();
    }

    public async Task<double> GetAverageViewsPerProfileAsync(DateTime? fromDate = null)
    {
        var query = Context.UserProfileViews.AsQueryable();
        
        if (fromDate.HasValue)
        {
            query = query.Where(upv => upv.ViewedAt >= fromDate.Value);
        }

        var profileViewCounts = await query
            .GroupBy(upv => upv.ProfileUserId)
            .Select(g => g.Count())
            .ToListAsync();

        return profileViewCounts.Any() ? profileViewCounts.Average() : 0.0;
    }

    public async Task<Dictionary<Guid, int>> GetViewStatisticsAsync(IEnumerable<Guid> profileUserIds)
    {
        return await Context.UserProfileViews
            .Where(upv => profileUserIds.Contains(upv.ProfileUserId))
            .GroupBy(upv => upv.ProfileUserId)
            .Select(g => new { ProfileUserId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.ProfileUserId, x => x.Count);
    }

    public async Task<bool> CleanupOldViewsAsync(DateTime cutoffDate)
    {
        var oldViews = await Context.UserProfileViews
            .Where(upv => upv.ViewedAt < cutoffDate)
            .ToListAsync();

        if (oldViews.Any())
        {
            Context.UserProfileViews.RemoveRange(oldViews);
            return true;
        }

        return false;
    }

    #endregion

    #region View Privacy

    public async Task<bool> IsViewTrackingEnabledAsync(Guid profileUserId)
    {
        // This would typically check user privacy settings
        // For now, we'll assume tracking is enabled by default
        return true;
    }

    public async Task<bool> SetViewTrackingAsync(Guid profileUserId, bool enabled)
    {
        // This would typically update user privacy settings
        // Implementation depends on how privacy settings are stored
        return true;
    }

    public async Task<IEnumerable<UserProfileView>> GetAnonymousViewsAsync(Guid profileUserId, int page = 1, int pageSize = 20)
    {
        return await Context.UserProfileViews
            .Where(upv => upv.ProfileUserId == profileUserId && upv.IsAnonymous)
            .OrderByDescending(upv => upv.ViewedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    #endregion
}