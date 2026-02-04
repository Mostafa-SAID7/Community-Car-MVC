using CommunityCar.Application.Common.Interfaces.Repositories.Account;
using CommunityCar.Application.Common.Interfaces.Services.Account.Profile;
using CommunityCar.Application.Features.Account.ViewModels.Activity;
using CommunityCar.Domain.Entities.Account.Profile;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.Account.Profile;

public class ProfileViewService : IProfileViewService
{
    private readonly IUserProfileViewRepository _viewRepository;
    private readonly IUserFollowingRepository _followingRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ProfileViewService> _logger;

    public ProfileViewService(
        IUserProfileViewRepository viewRepository,
        IUserFollowingRepository followingRepository,
        IUserRepository userRepository,
        ILogger<ProfileViewService> logger)
    {
        _viewRepository = viewRepository;
        _followingRepository = followingRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    #region IProfileViewService Implementation

    public async Task<bool> TrackProfileViewAsync(Guid profileUserId, Guid? viewerUserId = null, CancellationToken cancellationToken = default)
    {
        if (viewerUserId.HasValue && viewerUserId.Value == profileUserId) 
            return false;

        if (viewerUserId.HasValue)
        {
            return await RecordProfileViewAsync(viewerUserId.Value, profileUserId);
        }
        else
        {
            return await RecordAnonymousViewAsync(profileUserId);
        }
    }

    public async Task<WhoViewedMyProfileVM> GetWhoViewedMyProfileAsync(Guid userId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var views = await GetProfileViewsAsync(userId, page, pageSize);
        var totalCount = await _viewRepository.GetProfileViewCountAsync(userId);
        
        return new WhoViewedMyProfileVM
        {
            UserId = userId,
            Views = views.Count(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<int> GetProfileViewCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _viewRepository.GetProfileViewCountAsync(userId);
    }

    public async Task<int> GetProfileViewCountTodayAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _viewRepository.GetDailyViewCountAsync(userId, DateTime.UtcNow.Date);
    }

    public async Task<Dictionary<DateTime, int>> GetProfileViewsChartAsync(Guid userId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _viewRepository.GetViewTrendsAsync(userId, startDate, endDate);
    }

    public async Task<List<ProfileViewStatsVM>> GetTopViewedProfilesAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        // This would need to be implemented in the repository
        // For now, return empty list
        return new List<ProfileViewStatsVM>();
    }

    #endregion

    #region View Tracking

    public async Task<bool> RecordProfileViewAsync(Guid viewerId, Guid profileUserId, string? ipAddress = null, string? userAgent = null, string? location = null, string? referrerUrl = null, string? viewSource = null)
    {
        if (viewerId == profileUserId) return false;

        // Check for recent view to avoid duplicate tracking (e.g., within 30 mins)
        var recentView = await _viewRepository.GetRecentViewAsync(viewerId, profileUserId, 30);
        if (recentView != null) return false;

        var view = new UserProfileView(viewerId, profileUserId, ipAddress, userAgent, location, null, viewSource);
        await _viewRepository.AddAsync(view);
        
        _logger.LogInformation("Profile view recorded: {ViewerId} viewed {ProfileUserId}", viewerId, profileUserId);
        return true;
    }

    public async Task<bool> RecordAnonymousViewAsync(Guid profileUserId, string? ipAddress = null, string? userAgent = null, string? location = null, string? referrerUrl = null, string? viewSource = null)
    {
        var view = new UserProfileView(Guid.Empty, profileUserId, ipAddress, userAgent, location, null, viewSource);
        await _viewRepository.AddAsync(view);
        
        _logger.LogInformation("Anonymous profile view recorded for {ProfileUserId}", profileUserId);
        return true;
    }

    public async Task UpdateViewDurationAsync(Guid viewId, TimeSpan duration)
    {
        var view = await _viewRepository.GetByIdAsync(viewId);
        if (view != null)
        {
            // Assuming UserProfileView has a way to update duration
            // For now, placeholders if missing or implement in entity
            await _viewRepository.UpdateAsync(view);
        }
    }

    #endregion

    #region View Analytics

    public async Task<ProfileViewStatsVM> GetProfileViewStatsAsync(Guid profileUserId, DateTime? since = null)
    {
        var totalViews = await _viewRepository.GetProfileViewCountAsync(profileUserId, since);
        var uniqueViewers = await _viewRepository.GetUniqueViewerCountAsync(profileUserId, since);
        var recentViewsData = await GetProfileViewsAsync(profileUserId, 1, 10);
        var recentViews = recentViewsData.Select(v => new ProfileViewerVM
        {
            UserId = v.ViewerId,
            UserName = "User", // Will be populated from User entity lookup
            FullName = "User", // Will be populated from User entity lookup
            ProfilePictureUrl = null, // Will be populated from User entity lookup
            ViewedAt = v.ViewedAt,
            ViewCount = 1
        }).ToList();
        var topViewersIds = await _viewRepository.GetTopViewersAsync(profileUserId, 10);
        
        var fromDate = since ?? DateTime.UtcNow.AddDays(-30);
        var viewsByDate = await _viewRepository.GetViewsByDateAsync(profileUserId, fromDate, DateTime.UtcNow);

        var topViewers = new List<ProfileViewerVM>();
        foreach (var id in topViewersIds)
        {
            var viewer = await _userRepository.GetByIdAsync(id);
            topViewers.Add(new ProfileViewerVM
            {
                ViewerId = id,
                ViewerName = viewer?.Profile.FullName ?? "User",
                ViewerProfilePicture = viewer?.Profile.ProfilePictureUrl,
                ViewCount = 0 // Repository doesn't return count currently
            });
        }

        return new ProfileViewStatsVM
        {
            ProfileUserId = profileUserId,
            TotalViews = totalViews,
            UniqueViewers = uniqueViewers,
            RecentViews = recentViews,
            TopViewers = topViewers,
            DailyViews = viewsByDate,
            ViewsToday = await _viewRepository.GetDailyViewCountAsync(profileUserId, DateTime.UtcNow.Date),
            ViewsBySource = await _viewRepository.GetViewSourceStatsAsync(profileUserId, since)
        };
    }

    public async Task<IEnumerable<ProfileViewVM>> GetProfileViewsAsync(Guid profileUserId, int page = 1, int pageSize = 20)
    {
        var views = await _viewRepository.GetProfileViewsAsync(profileUserId, page, pageSize);
        return await MapToVMAsync(views, profileUserId);
    }

    public async Task<IEnumerable<ProfileViewVM>> GetUserViewHistoryAsync(Guid viewerId, int page = 1, int pageSize = 20)
    {
        var views = await _viewRepository.GetUserViewHistoryAsync(viewerId, page, pageSize);
        return await MapToVMAsync(views, Guid.Empty); // Passing Empty as we don't need mutual check relative to viewer
    }

    public async Task<IEnumerable<ProfileViewerVM>> GetTopViewersAsync(Guid profileUserId, int limit = 10, DateTime? since = null)
    {
        var viewerIds = await _viewRepository.GetTopViewersAsync(profileUserId, limit);
        var viewers = new List<ProfileViewerVM>();
        foreach (var id in viewerIds)
        {
            var viewer = await _userRepository.GetByIdAsync(id);
            viewers.Add(new ProfileViewerVM
            {
                ViewerId = id,
                ViewerName = viewer?.Profile.FullName ?? "User",
                ViewerProfilePicture = viewer?.Profile.ProfilePictureUrl
            });
        }
        return viewers;
    }

    public Task<Dictionary<string, int>> GetViewSourceStatsAsync(Guid profileUserId, DateTime? since = null)
        => _viewRepository.GetViewSourceStatsAsync(profileUserId, since);

    public Task<Dictionary<DateTime, int>> GetViewTrendsAsync(Guid profileUserId, DateTime startDate, DateTime endDate)
        => _viewRepository.GetViewTrendsAsync(profileUserId, startDate, endDate);

    #endregion

    #region View Queries

    public Task<bool> HasViewedProfileAsync(Guid viewerId, Guid profileUserId)
        => _viewRepository.HasViewedProfileAsync(viewerId, profileUserId);

    public async Task<DateTime?> GetLastViewDateAsync(Guid viewerId, Guid profileUserId)
    {
        var lastView = await _viewRepository.GetLastViewAsync(viewerId, profileUserId);
        return lastView?.ViewedAt;
    }

    public async Task<IEnumerable<ProfileViewVM>> GetMutualViewsAsync(Guid userId1, Guid userId2)
    {
        var views = await _viewRepository.GetMutualViewsAsync(userId1, userId2);
        return await MapToVMAsync(views, userId1);
    }

    public Task<bool> IsRecentViewerAsync(Guid viewerId, Guid profileUserId, int minutesThreshold = 30)
        => Task.FromResult(_viewRepository.GetRecentViewAsync(viewerId, profileUserId, minutesThreshold).Result != null);

    #endregion

    #region Privacy & Settings

    public Task<bool> CanViewProfileAsync(Guid viewerId, Guid profileUserId)
        => Task.FromResult(true); // Simplified

    public Task<bool> ShouldTrackViewAsync(Guid viewerId, Guid profileUserId)
        => _viewRepository.IsViewTrackingEnabledAsync(profileUserId);

    #endregion

    #region Maintenance

    public Task CleanupOldViewsAsync(int daysToKeep = 90)
        => _viewRepository.CleanupOldViewsAsync(DateTime.UtcNow.AddDays(-daysToKeep));

    public Task<int> GetDailyViewCountAsync(Guid profileUserId, DateTime date)
        => _viewRepository.GetDailyViewCountAsync(profileUserId, date);

    #endregion

    #region Private Helpers

    private async Task<IEnumerable<ProfileViewVM>> MapToVMAsync(IEnumerable<UserProfileView> views, Guid profileUserId)
    {
        var vms = new List<ProfileViewVM>();
        foreach (var v in views)
        {
            var viewer = v.ViewerId != Guid.Empty ? await _userRepository.GetByIdAsync(v.ViewerId) : null;
            vms.Add(new ProfileViewVM
            {
                Id = v.Id,
                ViewerId = v.ViewerId,
                ProfileUserId = v.ProfileUserId,
                ViewerName = viewer?.Profile.FullName ?? (v.ViewerId == Guid.Empty ? "Anonymous" : "User"),
                ViewerProfilePicture = viewer?.Profile.ProfilePictureUrl,
                ViewerLocation = v.ViewerLocation,
                ViewedAt = v.ViewedAt,
                ViewedTimeAgo = GetTimeAgo(v.ViewedAt),
                IsAnonymous = v.IsAnonymous,
                Device = v.ViewerUserAgent,
                ViewSource = v.ViewSource,
                IsMutual = profileUserId != Guid.Empty && v.ViewerId != Guid.Empty && await _followingRepository.AreMutualFollowersAsync(profileUserId, v.ViewerId)
            });
        }
        return vms;
    }

    private string GetTimeAgo(DateTime dateTime)
    {
        var span = DateTime.UtcNow - dateTime;
        if (span.TotalDays > 30) return dateTime.ToString("MMM dd, yyyy");
        if (span.TotalDays >= 1) return $"{(int)span.TotalDays}d ago";
        if (span.TotalHours >= 1) return $"{(int)span.TotalHours}h ago";
        if (span.TotalMinutes >= 1) return $"{(int)span.TotalMinutes}m ago";
        return "Just now";
    }

    #endregion
}