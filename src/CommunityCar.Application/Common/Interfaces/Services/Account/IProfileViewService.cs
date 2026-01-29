using CommunityCar.Application.Features.Account.ViewModels.Social;

namespace CommunityCar.Application.Common.Interfaces.Services.Account;


/// <summary>
/// Interface for profile view tracking and analytics
/// </summary>
public interface IProfileViewService
{
    // View Tracking
    Task<bool> RecordProfileViewAsync(Guid viewerId, Guid profileUserId, string? ipAddress = null, string? userAgent = null, string? location = null, string? referrerUrl = null, string? viewSource = null);
    Task<bool> RecordAnonymousViewAsync(Guid profileUserId, string? ipAddress = null, string? userAgent = null, string? location = null, string? referrerUrl = null, string? viewSource = null);
    Task UpdateViewDurationAsync(Guid viewId, TimeSpan duration);

    // View Analytics
    Task<ProfileViewStatsVM> GetProfileViewStatsAsync(Guid profileUserId, DateTime? since = null);
    Task<IEnumerable<ProfileViewVM>> GetProfileViewsAsync(Guid profileUserId, int page = 1, int pageSize = 20);
    Task<IEnumerable<ProfileViewVM>> GetUserViewHistoryAsync(Guid viewerId, int page = 1, int pageSize = 20);
    Task<IEnumerable<ProfileViewerVM>> GetTopViewersAsync(Guid profileUserId, int limit = 10, DateTime? since = null);
    Task<Dictionary<string, int>> GetViewSourceStatsAsync(Guid profileUserId, DateTime? since = null);
    Task<Dictionary<DateTime, int>> GetViewTrendsAsync(Guid profileUserId, DateTime startDate, DateTime endDate);

    // View Queries
    Task<bool> HasViewedProfileAsync(Guid viewerId, Guid profileUserId);
    Task<DateTime?> GetLastViewDateAsync(Guid viewerId, Guid profileUserId);
    Task<IEnumerable<ProfileViewVM>> GetMutualViewsAsync(Guid userId1, Guid userId2);
    Task<bool> IsRecentViewerAsync(Guid viewerId, Guid profileUserId, int minutesThreshold = 30);

    // Privacy & Settings
    Task<bool> CanViewProfileAsync(Guid viewerId, Guid profileUserId);
    Task<bool> ShouldTrackViewAsync(Guid viewerId, Guid profileUserId);

    // Maintenance
    Task CleanupOldViewsAsync(int daysToKeep = 90);
    Task<int> GetDailyViewCountAsync(Guid profileUserId, DateTime date);
}