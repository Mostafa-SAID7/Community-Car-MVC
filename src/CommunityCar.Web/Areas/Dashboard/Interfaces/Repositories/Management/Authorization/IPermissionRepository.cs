using CommunityCar.Domain.Entities.Account.Authorization;

namespace CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories.Management.Authorization;

public interface IPermissionRepository
{
    // Permission CRUD
    Task<Permission?> GetByIdAsync(Guid id);
    Task<Permission?> GetByNameAsync(string name);
    Task<IEnumerable<Permission>> GetAllAsync();
    Task<IEnumerable<Permission>> GetByCategoryAsync(string category);
    Task<IEnumerable<Permission>> GetActiveAsync();
    Task<Permission> AddAsync(Permission permission);
    Task<Permission> UpdateAsync(Permission permission);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(string name);

    // User Permissions
    Task<IEnumerable<UserPermission>> GetUserPermissionsAsync(Guid userId);
    Task<IEnumerable<string>> GetUserPermissionNamesAsync(Guid userId);
    Task<UserPermission?> GetUserPermissionAsync(Guid userId, Guid permissionId);
    Task<UserPermission?> GetUserPermissionByNameAsync(Guid userId, string permissionName);
    Task<UserPermission> AddUserPermissionAsync(UserPermission userPermission);
    Task<UserPermission> UpdateUserPermissionAsync(UserPermission userPermission);
    Task<bool> RemoveUserPermissionAsync(Guid userId, Guid permissionId);
    Task<bool> RemoveAllUserPermissionsAsync(Guid userId);

    // Role Permissions
    Task<IEnumerable<RolePermission>> GetRolePermissionsAsync(Guid roleId);
    Task<IEnumerable<string>> GetRolePermissionNamesAsync(Guid roleId);
    Task<RolePermission?> GetRolePermissionAsync(Guid roleId, Guid permissionId);
    Task<RolePermission?> GetRolePermissionByNameAsync(string roleName, string permissionName);
    Task<RolePermission> AddRolePermissionAsync(RolePermission rolePermission);
    Task<RolePermission> UpdateRolePermissionAsync(RolePermission rolePermission);
    Task<bool> RemoveRolePermissionAsync(Guid roleId, Guid permissionId);
    Task<bool> RemoveAllRolePermissionsAsync(Guid roleId);

    // Permission Queries
    Task<bool> UserHasPermissionAsync(Guid userId, string permissionName);
    Task<bool> RoleHasPermissionAsync(Guid roleId, string permissionName);
    Task<IEnumerable<Guid>> GetUsersWithPermissionAsync(string permissionName);
    Task<IEnumerable<Guid>> GetRolesWithPermissionAsync(string permissionName);
    Task<IEnumerable<string>> GetEffectiveUserPermissionsAsync(Guid userId);

    // Bulk Operations
    Task<bool> BulkAddUserPermissionsAsync(Guid userId, IEnumerable<UserPermission> permissions);
    Task<bool> BulkRemoveUserPermissionsAsync(Guid userId, IEnumerable<string> permissionNames);
    Task<bool> BulkAddRolePermissionsAsync(Guid roleId, IEnumerable<RolePermission> permissions);
    Task<bool> BulkRemoveRolePermissionsAsync(Guid roleId, IEnumerable<string> permissionNames);

    // Statistics
    Task<int> GetPermissionCountAsync();
    Task<int> GetUserPermissionCountAsync(Guid userId);
    Task<int> GetRolePermissionCountAsync(Guid roleId);
    Task<Dictionary<string, int>> GetPermissionCountByCategoryAsync();
}


