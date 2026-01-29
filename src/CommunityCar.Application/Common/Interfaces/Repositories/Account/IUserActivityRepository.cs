using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Account.Core;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Account;

/// <summary>
/// Repository interface for UserActivity entity operations
/// </summary>
public interface IUserActivityRepository : IBaseRepository<UserActivity>
{
    #region Activity Tracking
    Task<IEnumerable<UserActivity>> GetUserActivitiesAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<IEnumerable<UserActivity>> GetRecentActivitiesAsync(Guid userId, int count = 10);
    Task<IEnumerable<UserActivity>> GetActivitiesByTypeAsync(Guid userId, string activityType);
    Task<IEnumerable<UserActivity>> GetActivitiesInDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate);
    #endregion

    #region Activity Analytics
    Task<int> GetActivityCountAsync(Guid userId, DateTime? fromDate = null);
    Task<Dictionary<string, int>> GetActivityCountByTypeAsync(Guid userId, DateTime? fromDate = null);
    Task<IEnumerable<UserActivity>> GetMostActiveUsersAsync(DateTime fromDate, int count = 10);
    Task<DateTime?> GetLastActivityDateAsync(Guid userId);
    #endregion

    #region Activity Management
    Task LogActivityAsync(Guid userId, string activityType, string description, Dictionary<string, object>? metadata = null);
    Task<bool> DeleteOldActivitiesAsync(DateTime cutoffDate);
    Task<bool> DeleteUserActivitiesAsync(Guid userId);
    #endregion
}