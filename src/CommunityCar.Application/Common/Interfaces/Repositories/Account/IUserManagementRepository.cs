using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Account.Management;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Account;

/// <summary>
/// Repository interface for UserManagement entity operations
/// </summary>
public interface IUserManagementRepository : IBaseRepository<UserManagement>
{
    #region User Management Operations
    Task<UserManagement?> GetUserManagementAsync(Guid userId);
    Task<IEnumerable<UserManagement>> GetManagedUsersAsync(Guid managerId);
    Task<bool> AssignManagerAsync(Guid userId, Guid managerId);
    Task<bool> RemoveManagerAsync(Guid userId);
    Task<bool> TransferManagementAsync(Guid userId, Guid newManagerId);
    #endregion

    #region Management Hierarchy
    Task<IEnumerable<UserManagement>> GetManagementHierarchyAsync(Guid rootManagerId);
    Task<int> GetManagedUserCountAsync(Guid managerId);
    Task<bool> IsManagerAsync(Guid userId);
    Task<bool> CanManageUserAsync(Guid managerId, Guid userId);
    #endregion

    #region Management Analytics
    Task<Dictionary<Guid, int>> GetManagementStatisticsAsync();
    Task<IEnumerable<UserManagement>> GetTopManagersAsync(int count = 10);
    Task<IEnumerable<UserManagement>> GetRecentManagementChangesAsync(int count = 20);
    #endregion
}