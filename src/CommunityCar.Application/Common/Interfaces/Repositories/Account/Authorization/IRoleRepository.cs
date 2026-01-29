using CommunityCar.Domain.Entities.Account.Authorization;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Authorization;

public interface IRoleRepository
{
    // Role CRUD
    Task<Role?> GetByIdAsync(Guid id);
    Task<Role?> GetByNameAsync(string name);
    Task<IEnumerable<Role>> GetAllAsync();
    Task<IEnumerable<Role>> GetByCategoryAsync(string category);
    Task<IEnumerable<Role>> GetActiveAsync();
    Task<IEnumerable<Role>> GetSystemRolesAsync();
    Task<Role> AddAsync(Role role);
    Task<Role> UpdateAsync(Role role);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(string name);

    // User Roles (using Identity)
    Task<IEnumerable<string>> GetUserRolesAsync(Guid userId);
    Task<IEnumerable<Role>> GetUserRoleDetailsAsync(Guid userId);
    Task<bool> IsUserInRoleAsync(Guid userId, string roleName);
    Task<bool> AddUserToRoleAsync(Guid userId, string roleName);
    Task<bool> RemoveUserFromRoleAsync(Guid userId, string roleName);
    Task<bool> AddUserToRolesAsync(Guid userId, IEnumerable<string> roleNames);
    Task<bool> RemoveUserFromRolesAsync(Guid userId, IEnumerable<string> roleNames);
    Task<IEnumerable<Guid>> GetUsersInRoleAsync(string roleName);

    // Role Hierarchy
    Task<IEnumerable<Role>> GetRolesByPriorityAsync();
    Task<Role?> GetHighestPriorityUserRoleAsync(Guid userId);
    Task<bool> UpdateRolePriorityAsync(Guid roleId, int priority);

    // Role Statistics
    Task<int> GetRoleCountAsync();
    Task<int> GetUserCountInRoleAsync(string roleName);
    Task<Dictionary<string, int>> GetRoleCountByCategoryAsync();
    Task<Dictionary<string, int>> GetUserCountByRoleAsync();

    // Bulk Operations
    Task<bool> BulkAssignRolesAsync(Guid userId, IEnumerable<string> roleNames);
    Task<bool> BulkRemoveRolesAsync(Guid userId, IEnumerable<string> roleNames);
    Task<bool> SyncUserRolesAsync(Guid userId, IEnumerable<string> roleNames);
}