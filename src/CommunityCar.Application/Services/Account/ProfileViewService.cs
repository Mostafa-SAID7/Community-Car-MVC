using CommunityCar.Application.Common.Interfaces.Repositories.Account;
using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Common.Interfaces.Services;
using CommunityCar.Application.Features.Account.ViewModels.Social;
using CommunityCar.Domain.Entities.Account.Profile;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.Account;

public class ProfileViewService : IProfileViewService
{
    private readonly IUserProfileViewRepository _profileViewRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserFollowingRepository _followingRepository;
    private readonly ILogger<ProfileViewService> _logger;

    public ProfileViewService(
        IUserProfileViewRepository profileViewRepository,
        IUserRepository userRepository,
        IUserFollowingRepository followingRepository,
        ILogger<ProfileViewService> logger)
    {
        _profileViewRepository = profileViewRepository;
        _userRepository = userRepository;
        _followingRepository = followingRepository;
        _logger = logger;
    }

    public async Task<bool> RecordProfileViewAsync(Guid viewerId, Guid profileUserId, string? ipAddress = null, string? userAgent = null, string? location = null, string? referrerUrl = null, string? viewSource = null)
    {
        try
        {
            if (viewerId == profileUserId) return false;

            if (!await ShouldTrackViewAsync(viewerId, profileUserId))
                return false;

            var recentView = await _profileViewRepository.GetRecentViewAsync(viewerId, profileUserId, 30);
            if (recentView != null) return false;

            var profileView = new UserProfileView(viewerId, profileUserId, ipAddress, userAgent, location, referrerUrl, viewSource);
            await _profileViewRepository.AddAsync(profileView);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording profile view");
            return false;
        }
    }

    public async Task<bool> RecordAnonymousViewAsync(Guid profileUserId, string? ipAddress = null, string? userAgent = null, string? location = null, string? referrerUrl = null, string? viewSource = null)
    {
        try
        {
            var profileView = new UserProfileView(profileUserId, ipAddress, userAgent, location, referrerUrl, viewSource);
            await _profileViewRepository.AddAsync(profileView);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording anonymous view");
            return false;
        }
    }

    public async Task UpdateViewDurationAsync(Guid viewId, TimeSpan duration)
    {
        var view = await _profileViewRepository.GetByIdAsync(viewId);
        if (view != null)
        {
            view.UpdateViewDuration(duration);
            await _profileViewRepository.UpdateAsync(view);
        }
    }

    public async Task<ProfileViewStatsVM> GetProfileViewStatsAsync(Guid profileUserId, DateTime? since = null)
    {
        var totalViews = await _profileViewRepository.GetProfileViewCountAsync(profileUserId, since);
        var uniqueViewers = await _profileViewRepository.GetUniqueViewerCountAsync(profileUserId, since);
        var todayViews = await _profileViewRepository.GetDailyViewCountAsync(profileUserId, DateTime.Today);
        var weekViews = await _profileViewRepository.GetProfileViewCountAsync(profileUserId, DateTime.Today.AddDays(-7));
        var monthViews = await _profileViewRepository.GetProfileViewCountAsync(profileUserId, DateTime.Today.AddDays(-30));
        var viewSourceStats = await _profileViewRepository.GetViewSourceStatsAsync(profileUserId, since);

        return new ProfileViewStatsVM
        {
            ProfileUserId = profileUserId,
            TotalViews = totalViews,
            UniqueViewers = uniqueViewers,
            ViewsToday = todayViews,
            ViewsThisWeek = weekViews,
            ViewsThisMonth = monthViews,
            ViewSourceBreakdown = viewSourceStats,
            MostCommonViewSource = viewSourceStats.OrderByDescending(x => x.Value).FirstOrDefault().Key ?? "Direct"
        };
    }

    public async Task<IEnumerable<ProfileViewVM>> GetProfileViewsAsync(Guid profileUserId, int page = 1, int pageSize = 20)
    {
        var views = await _profileViewRepository.GetProfileViewsAsync(profileUserId, page, pageSize);
        var result = new List<ProfileViewVM>();

        foreach (var view in views)
        {
            var vm = MapToVM(view);
            if (!view.IsAnonymous)
            {
                var viewer = await _userRepository.GetByIdAsync(view.ViewerId);
                if (viewer != null)
                {
                    vm.ViewerName = viewer.Profile.FullName;
                    vm.ViewerProfilePicture = viewer.Profile.ProfilePictureUrl;
                }
            }
            result.Add(vm);
        }
        return result;
    }

    public async Task<IEnumerable<ProfileViewVM>> GetUserViewHistoryAsync(Guid viewerId, int page = 1, int pageSize = 20)
    {
        var views = await _profileViewRepository.GetUserViewHistoryAsync(viewerId, page, pageSize);
        var result = new List<ProfileViewVM>();

        foreach (var view in views)
        {
            var vm = MapToVM(view);
            var profileUser = await _userRepository.GetByIdAsync(view.ProfileUserId);
            if (profileUser != null)
            {
                vm.ViewerName = profileUser.Profile.FullName;
                vm.ViewerProfilePicture = profileUser.Profile.ProfilePictureUrl;
            }
            result.Add(vm);
        }
        return result;
    }

    public async Task<IEnumerable<ProfileViewerVM>> GetTopViewersAsync(Guid profileUserId, int limit = 10, DateTime? since = null)
    {
        var topViewerIds = await _profileViewRepository.GetTopViewersAsync(profileUserId, limit);
        var result = new List<ProfileViewerVM>();

        foreach (var viewerId in topViewerIds)
        {
            var viewer = await _userRepository.GetByIdAsync(viewerId);
            if (viewer != null)
            {
                var viewCount = await _profileViewRepository.GetProfileViewCountAsync(profileUserId, since); // This is inefficient but keep it simple for now
                var lastView = await _profileViewRepository.GetLastViewAsync(viewerId, profileUserId);
                
                result.Add(new ProfileViewerVM
                {
                    ViewerId = viewerId,
                    ViewerName = viewer.Profile.FullName,
                    ViewerProfilePicture = viewer.Profile.ProfilePictureUrl,
                    ViewCount = viewCount,
                    LastViewedAt = lastView?.ViewedAt ?? DateTime.MinValue,
                    IsFollowing = await _followingRepository.IsFollowingAsync(viewerId, profileUserId)
                });
            }
        }
        return result;
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
        var lastView = await _profileViewRepository.GetLastViewAsync(viewerId, profileUserId);
        return lastView?.ViewedAt;
    }

    public async Task<IEnumerable<ProfileViewVM>> GetMutualViewsAsync(Guid userId1, Guid userId2)
    {
        var views = await _profileViewRepository.GetMutualViewsAsync(userId1, userId2);
        return views.Select(MapToVM).ToList();
    }

    public async Task<bool> IsRecentViewerAsync(Guid viewerId, Guid profileUserId, int minutesThreshold = 30)
    {
        var recentView = await _profileViewRepository.GetRecentViewAsync(viewerId, profileUserId, minutesThreshold);
        return recentView != null;
    }

    public async Task<bool> CanViewProfileAsync(Guid viewerId, Guid profileUserId)
    {
        var profileUser = await _userRepository.GetByIdAsync(profileUserId);
        if (profileUser == null || !profileUser.IsActive) return false;
        if (profileUser.PrivacySettings.IsPublic) return true;
        if (viewerId == profileUserId) return true;

        return await _followingRepository.IsFollowingAsync(profileUserId, viewerId);
    }

    public async Task<bool> ShouldTrackViewAsync(Guid viewerId, Guid profileUserId)
    {
        if (viewerId == profileUserId) return false;
        var profileUser = await _userRepository.GetByIdAsync(profileUserId);
        return profileUser != null && profileUser.IsActive;
    }

    public async Task CleanupOldViewsAsync(int daysToKeep = 90)
    {
        await _profileViewRepository.CleanupOldViewsAsync(DateTime.UtcNow.AddDays(-daysToKeep));
    }

    public async Task<int> GetDailyViewCountAsync(Guid profileUserId, DateTime date)
    {
        return await _profileViewRepository.GetDailyViewCountAsync(profileUserId, date);
    }

    private ProfileViewVM MapToVM(UserProfileView view)
    {
        return new ProfileViewVM
        {
            Id = view.Id,
            ViewerId = view.ViewerId,
            ProfileUserId = view.ProfileUserId,
            ViewedAt = view.ViewedAt,
            IsAnonymous = view.IsAnonymous,
            Location = view.Location ?? string.Empty,
            ViewerLocation = view.ViewerLocation,
            DeviceType = view.DeviceType,
            ViewSource = view.ViewSource,
            ViewDuration = view.ViewDuration.TotalSeconds,
            ReferrerUrl = view.ReferrerUrl
        };
    }
}