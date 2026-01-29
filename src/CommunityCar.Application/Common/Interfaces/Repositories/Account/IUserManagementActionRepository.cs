using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Account.Management;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Account;

/// <summary>
/// Repository interface for UserManagementAction entity operations
/// </summary>
public interface IUserManagementActionRepository : IBaseRepository<UserManagementAction>
{
    #region Management Action Tracking
    Task<IEnumerable<UserManagementAction>> GetUserActionsAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<IEnumerable<UserManagementAction>> GetManagerActionsAsync(Guid managerId, int page = 1, int pageSize = 20);
    Task<IEnumerable<UserManagementAction>> GetActionsByTypeAsync(string actionType, int page = 1, int pageSize = 20);
    Task LogManagementActionAsync(Guid managerId, Guid targetUserId, string actionType, string description, Dictionary<string, object>? metadata = null);
    #endregion

    #region Action Analytics
    Task<int> GetActionCountAsync(Guid? managerId = null, Guid? targetUserId = null, DateTime? fromDate = null);
    Task<Dictionary<string, int>> GetActionCountByTypeAsync(Guid? managerId = null, DateTime? fromDate = null);
    Task<IEnumerable<UserManagementAction>> GetRecentActionsAsync(int count = 20);
    Task<DateTime?> GetLastActionDateAsync(Guid managerId);
    #endregion

    #region Action Audit
    Task<IEnumerable<UserManagementAction>> GetAuditTrailAsync(Guid userId, DateTime? fromDate = null);
    Task<bool> DeleteOldActionsAsync(DateTime cutoffDate);
    Task<IEnumerable<UserManagementAction>> SearchActionsAsync(string searchTerm, int page = 1, int pageSize = 20);
    #endregion
}