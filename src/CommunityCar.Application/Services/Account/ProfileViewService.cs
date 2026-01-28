using CommunityCar.Application.Common.Interfaces.Repositories.Profile;
using CommunityCar.Application.Common.Interfaces.Repositories.User;
using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Domain.Entities.Account.Profile;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.Account;

public class ProfileViewService : IProfileViewService
{
    private readonly IUserProfileViewRepository _profileViewRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserFollowingRepository _followingRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ProfileViewService> _logger;

    public ProfileViewService(
        IUserProfileViewRepository profileViewRepository,
        IUserRepository userRepository,
        IUserFollowingRepository followingRepository,
        ICurrentUserService currentUserService,
        ILogger<ProfileViewService> logger)
    {
        _profileViewRepository = profileViewRepository;
        _userRepository = userRepository;
        _followingRepository = followingRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<bool> RecordProfileViewAsync(Guid viewerId, Guid profileUserId, string? ipAddress = null, string? userAgent = null, string? location = null, string? referrerUrl = null, string? viewSource = null)
    {
        try
        {
            // Don't track self-views
            if (viewerId == profileUserId)
                return false;

            // Check if we should track this view
            if (!await ShouldTrackViewAsync(viewerId, profileUserId))
                return false;

            // Check for recent view to avoid spam
            var recentView = await _profileViewRepository.GetRecentViewAsync(viewerId, profileUserId, 30);
            if (recentView != null)
            {
                _logger.LogDebug("Recent view found for viewer {ViewerId} on profile {ProfileUserId}, skipping", viewerId, profileUserId);
                return false;
            }

            var profileView = new UserProfileView(viewerId, profileUserId, ipAddress, userAgent, location, referrerUrl, viewSource);
            await _profileViewRepository.AddAsync(profileView);

            _logger.LogInformation("Profile view recorded: {ViewerId} viewed {ProfileUserId}", viewerId, profileUserId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording profile view for viewer {ViewerId} on profile {ProfileUserId}", viewerId, profileUserId);
            return false;
        }
    }

    public async Task<bool> RecordAnonymousViewAsync(Guid profileUserId, string? ipAddress = null, string? userAgent = null, string? location = null, string? referrerUrl = null, string? viewSource = null)
    {
        try
        {
            var profileView = new UserProfileView(profileUserId, ipAddress, userAgent, location, referrerUrl, viewSource);
            await _profileViewRepository.AddAsync(profileView);

            _logger.LogInformation("Anonymous profile view recorded for profile {ProfileUserId}", profileUserId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording anonymous profile view for profile {ProfileUserId}", profileUserId);
            return false;
        }
    }

    public async Task UpdateViewDurationAsync(Guid viewId, TimeSpan duration)
    {
        try
        {
            var view = await _profileViewRepository.GetByIdAsync(viewId);
            if (view != null)
            {
                view.UpdateViewDuration(duration);
                await _profileViewRepository.UpdateAsync(view);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating view duration for view {ViewId}", viewId);
        }
    }

    public async Task<ProfileViewStatsVM> GetProfileViewStatsAsync(Guid profileUserId, DateTime? since = null)
    {
        var totalViews = await _profileViewRepository.GetProfileViewCountAsync(profileUserId, since);
        var uniqueViewers = await _profileViewRepository.GetUniqueViewersCountAsync(profileUserId, since);
        var todayViews = await _profileViewRepository.GetDailyViewCountAsync(profileUserId, DateTime.Today);
        var weekViews = await _profileViewRepository.GetProfileViewCountAsync(profileUserId, DateTime.Today.AddDays(-7));
        var monthViews = await _profileViewRepository.GetProfileViewCountAsync(profileUserId, DateTime.Today.AddDays(-30));
        var viewSourceStats = await _profileViewRepository.GetViewSourceStatsAsync(profileUserId, since);

        var recentViews = await _profileViewRepository.GetProfileViewsAsync(profileUserId, 1, 1);
        var lastViewedAt = recentViews.FirstOrDefault()?.ViewedAt;

        var mostCommonSource = viewSourceStats.OrderByDescending(x => x.Value).FirstOrDefault().Key;

        return new ProfileViewStatsVM
        {
            TotalViews = totalViews,
            UniqueViewers = uniqueViewers,
            TodayViews = todayViews,
            WeekViews = weekViews,
            MonthViews = monthViews,
            LastViewedAt = lastViewedAt,
            MostCommonViewSource = mostCommonSource,
            ViewSourceBreakdown = viewSourceStats
        };
    }

    public async Task<IEnumerable<ProfileViewVM>> GetProfileViewsAsync(Guid profileUserId, int page = 1, int pageSize = 20)
    {
        var views = await _profileViewRepository.GetProfileViewsAsync(profileUserId, page, pageSize);
        var result = new List<ProfileViewVM>();

        foreach (var view in views)
        {
            var viewModel = new ProfileViewVM
            {
                Id = view.Id,
                ViewerId = view.ViewerId,
                ProfileUserId = view.ProfileUserId,
                ViewedAt = view.ViewedAt,
                ViewSource = view.ViewSource,
                ViewDuration = view.ViewDuration,
                IsAnonymous = view.IsAnonymous,
                ReferrerUrl = view.ReferrerUrl,
                ViewerLocation = view.ViewerLocation
            };

            if (!view.IsAnonymous)
            {
                var viewer = await _userRepository.GetByIdAsync(view.ViewerId);
                if (viewer != null)
                {
                    viewModel.ViewerName = viewer.Profile.FullName;
                    viewModel.ViewerProfilePicture = viewer.Profile.ProfilePictureUrl;
                }
            }

            result.Add(viewModel);
        }

        return result;
    }

    public async Task<IEnumerable<ProfileViewVM>> GetUserViewHistoryAsync(Guid viewerId, int page = 1, int pageSize = 20)
    {
        var views = await _profileViewRepository.GetUserViewHistoryAsync(viewerId, page, pageSize);
        var result = new List<ProfileViewVM>();

        foreach (var view in views)
        {
            var profileUser = await _userRepository.GetByIdAsync(view.ProfileUserId);
            var viewModel = new ProfileViewVM
            {
                Id = view.Id,
                ViewerId = view.ViewerId,
                ProfileUserId = view.ProfileUserId,
                ViewedAt = view.ViewedAt,
                ViewSource = view.ViewSource,
                ViewDuration = view.ViewDuration,
                IsAnonymous = view.IsAnonymous,
                ReferrerUrl = view.ReferrerUrl,
                ViewerLocation = view.ViewerLocation
            };

            if (profileUser != null)
            {
                viewModel.ViewerName = profileUser.Profile.FullName;
                viewModel.ViewerProfilePicture = profileUser.Profile.ProfilePictureUrl;
            }

            result.Add(viewModel);
        }

        return result;
    }

    public async Task<IEnumerable<ProfileViewerVM>> GetTopViewersAsync(Guid profileUserId, int limit = 10, DateTime? since = null)
    {
        var topViews = await _profileViewRepository.GetTopViewersAsync(profileUserId, limit, since);
        var result = new List<ProfileViewerVM>();

        var viewerGroups = topViews.GroupBy(v => v.ViewerId);

        foreach (var group in viewerGroups)
        {
            var viewerId = group.Key;
            var views = group.ToList();
            var viewer = await _userRepository.GetByIdAsync(viewerId);
            
            if (viewer != null)
            {
                var isFollowing = await _followingRepository.IsFollowingAsync(viewerId, profileUserId);
                var isMutualFollowing = isFollowing && await _followingRepository.IsFollowingAsync(profileUserId, viewerId);

                var viewerModel = new ProfileViewerVM
                {
                    ViewerId = viewerId,
                    ViewerName = viewer.Profile.FullName,
                    ViewerProfilePicture = viewer.Profile.ProfilePictureUrl,
                    ViewerLocation = viewer.Profile.City,
                    ViewCount = views.Count,
                    LastViewedAt = views.Max(v => v.ViewedAt),
                    FirstViewedAt = views.Min(v => v.ViewedAt),
                    IsFollowing = isFollowing,
                    IsMutualFollowing = isMutualFollowing,
                    AverageViewDuration = TimeSpan.FromSeconds(views.Average(v => v.ViewDuration.TotalSeconds))
                };

                result.Add(viewerModel);
            }
        }

        return result.OrderByDescending(v => v.ViewCount).ThenByDescending(v => v.LastViewedAt);
    }

    public async Task<Dictionary<string, int>> GetViewSourceStatsAsync(Guid profileUserId, DateTime? since = null)
    {
        return await _profileViewRepository.GetViewSourceStatsAsync(profileUserId, since);
    }

    public async Task<Dictionary<DateTime, int>> GetViewTrendsAsync(Guid profileUserId, DateTime startDate, DateTime endDate)
    {
        return await _profileViewRepository.GetViewTrendsAsync(profileUserId, startDate, endDate);
    }

    public async Task<bool> HasViewedProfileAsync(Guid viewerId, Guid profileUserId)
    {
        return await _profileViewRepository.HasViewedProfileAsync(viewerId, profileUserId);
    }

    public async Task<DateTime?> GetLastViewDateAsync(Guid viewerId, Guid profileUserId)
    {
        return await _profileViewRepository.GetLastViewDateAsync(viewerId, profileUserId);
    }

    public async Task<IEnumerable<ProfileViewVM>> GetMutualViewsAsync(Guid userId1, Guid userId2)
    {
        var views = await _profileViewRepository.GetMutualViewsAsync(userId1, userId2);
        var result = new List<ProfileViewVM>();

        foreach (var view in views)
        {
            var viewer = await _userRepository.GetByIdAsync(view.ViewerId);
            var profileUser = await _userRepository.GetByIdAsync(view.ProfileUserId);

            var viewModel = new ProfileViewVM
            {
                Id = view.Id,
                ViewerId = view.ViewerId,
                ProfileUserId = view.ProfileUserId,
                ViewedAt = view.ViewedAt,
                ViewSource = view.ViewSource,
                ViewDuration = view.ViewDuration,
                IsAnonymous = view.IsAnonymous,
                ReferrerUrl = view.ReferrerUrl,
                ViewerLocation = view.ViewerLocation
            };

            if (viewer != null)
            {
                viewModel.ViewerName = viewer.Profile.FullName;
                viewModel.ViewerProfilePicture = viewer.Profile.ProfilePictureUrl;
            }

            result.Add(viewModel);
        }

        return result;
    }

    public async Task<bool> IsRecentViewerAsync(Guid viewerId, Guid profileUserId, int minutesThreshold = 30)
    {
        var recentView = await _profileViewRepository.GetRecentViewAsync(viewerId, profileUserId, minutesThreshold);
        return recentView != null;
    }

    public async Task<bool> CanViewProfileAsync(Guid viewerId, Guid profileUserId)
    {
        var profileUser = await _userRepository.GetByIdAsync(profileUserId);
        if (profileUser == null || !profileUser.IsActive || profileUser.IsDeleted)
            return false;

        // If profile is public, anyone can view
        if (profileUser.PrivacySettings.IsPublic)
            return true;

        // If viewer is the profile owner, they can always view
        if (viewerId == profileUserId)
            return true;

        // Check if they are following each other for private profiles
        var isFollowing = await _followingRepository.IsFollowingAsync(profileUserId, viewerId);
        return isFollowing;
    }

    public async Task<bool> ShouldTrackViewAsync(Guid viewerId, Guid profileUserId)
    {
        // Don't track self-views
        if (viewerId == profileUserId)
            return false;

        // Check if profile user exists and is active
        var profileUser = await _userRepository.GetByIdAsync(profileUserId);
        if (profileUser == null || !profileUser.IsActive || profileUser.IsDeleted)
            return false;

        // Check if viewer exists and is active
        var viewer = await _userRepository.GetByIdAsync(viewerId);
        if (viewer == null || !viewer.IsActive || viewer.IsDeleted)
            return false;

        return true;
    }

    public async Task CleanupOldViewsAsync(int daysToKeep = 90)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);
        await _profileViewRepository.CleanupOldViewsAsync(cutoffDate);
        _logger.LogInformation("Cleaned up profile views older than {CutoffDate}", cutoffDate);
    }

    public async Task<int> GetDailyViewCountAsync(Guid profileUserId, DateTime date)
    {
        return await _profileViewRepository.GetDailyViewCountAsync(profileUserId, date);
    }
}