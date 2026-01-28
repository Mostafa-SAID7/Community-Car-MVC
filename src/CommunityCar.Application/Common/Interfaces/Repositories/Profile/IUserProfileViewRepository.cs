using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Domain.Entities.Account.Profile;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Profile;

public interface IUserProfileViewRepository : IRepository<UserProfileView>
{
    Task<IEnumerable<UserProfileView>> GetProfileViewsAsync(Guid profileUserId, int page = 1, int pageSize = 20);
    Task<IEnumerable<UserProfileView>> GetUserViewHistoryAsync(Guid viewerId, int page = 1, int pageSize = 20);
    Task<int> GetProfileViewCountAsync(Guid profileUserId, DateTime? since = null);
    Task<int> GetUniqueViewersCountAsync(Guid profileUserId, DateTime? since = null);
    Task<UserProfileView?> GetRecentViewAsync(Guid viewerId, Guid profileUserId, int minutesThreshold = 30);
    Task<IEnumerable<UserProfileView>> GetViewsBySourceAsync(Guid profileUserId, string viewSource, int page = 1, int pageSize = 20);
    Task<Dictionary<string, int>> GetViewSourceStatsAsync(Guid profileUserId, DateTime? since = null);
    Task<IEnumerable<UserProfileView>> GetTopViewersAsync(Guid profileUserId, int limit = 10, DateTime? since = null);
    Task<bool> HasViewedProfileAsync(Guid viewerId, Guid profileUserId);
    Task<DateTime?> GetLastViewDateAsync(Guid viewerId, Guid profileUserId);
    Task<IEnumerable<UserProfileView>> GetMutualViewsAsync(Guid userId1, Guid userId2);
    Task<int> GetDailyViewCountAsync(Guid profileUserId, DateTime date);
    Task<Dictionary<DateTime, int>> GetViewTrendsAsync(Guid profileUserId, DateTime startDate, DateTime endDate);
    Task CleanupOldViewsAsync(DateTime cutoffDate);
}