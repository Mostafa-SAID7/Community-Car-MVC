using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Account.Profile;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Account;

/// <summary>
/// Repository interface for UserProfileView entity operations
/// </summary>
public interface IUserProfileViewRepository : IBaseRepository<UserProfileView>
{
    #region Profile View Tracking
    Task<bool> RecordProfileViewAsync(Guid viewerId, Guid profileUserId, string? ipAddress = null, string? userAgent = null);
    Task<IEnumerable<UserProfileView>> GetProfileViewsAsync(Guid profileUserId, int page = 1, int pageSize = 20);
    Task<IEnumerable<UserProfileView>> GetUserViewHistoryAsync(Guid viewerId, int page = 1, int pageSize = 20);
    Task<UserProfileView?> GetLastViewAsync(Guid viewerId, Guid profileUserId);
    #endregion

    #region View Analytics
    Task<int> GetProfileViewCountAsync(Guid profileUserId, DateTime? fromDate = null);
    Task<int> GetUniqueViewerCountAsync(Guid profileUserId, DateTime? fromDate = null);
    Task<IEnumerable<UserProfileView>> GetRecentViewsAsync(Guid profileUserId, int count = 10);
    Task<Dictionary<DateTime, int>> GetViewsByDateAsync(Guid profileUserId, DateTime fromDate, DateTime toDate);
    Task<IEnumerable<Guid>> GetTopViewersAsync(Guid profileUserId, int count = 10);
    #endregion

    #region View Statistics
    Task<IEnumerable<Guid>> GetMostViewedProfilesAsync(int count = 10, DateTime? fromDate = null);
    Task<double> GetAverageViewsPerProfileAsync(DateTime? fromDate = null);
    Task<Dictionary<Guid, int>> GetViewStatisticsAsync(IEnumerable<Guid> profileUserIds);
    Task<bool> CleanupOldViewsAsync(DateTime cutoffDate);
    #endregion

    #region View Privacy
    Task<bool> IsViewTrackingEnabledAsync(Guid profileUserId);
    Task<bool> SetViewTrackingAsync(Guid profileUserId, bool enabled);
    Task<IEnumerable<UserProfileView>> GetAnonymousViewsAsync(Guid profileUserId, int page = 1, int pageSize = 20);
    #endregion

    #region New Tracking Methods
    Task<UserProfileView?> GetRecentViewAsync(Guid viewerId, Guid profileUserId, int minutesThreshold);
    Task<bool> HasViewedProfileAsync(Guid viewerId, Guid profileUserId);
    Task<IEnumerable<UserProfileView>> GetMutualViewsAsync(Guid userId1, Guid userId2);
    Task<Dictionary<string, int>> GetViewSourceStatsAsync(Guid profileUserId, DateTime? since = null);
    Task<Dictionary<DateTime, int>> GetViewTrendsAsync(Guid profileUserId, DateTime startDate, DateTime endDate);
    Task<int> GetDailyViewCountAsync(Guid profileUserId, DateTime date);
    #endregion
}