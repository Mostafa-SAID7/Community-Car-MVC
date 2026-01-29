using CommunityCar.Application.Features.Account.ViewModels.Authorization;

namespace CommunityCar.Application.Common.Interfaces.Services.Authorization;

/// <summary>
/// Service for managing permissions
/// </summary>
public interface IPermissionService
{
    // Permission Management
    Task<IEnumerable<PermissionVM>> GetAllPermissionsAsync();
    Task<IEnumerable<PermissionVM>> GetPermissionsByCategoryAsync(string category);
    Task<PermissionVM?> GetPermissionByNameAsync(string name);
    Task<PermissionVM?> GetPermissionByIdAsync(Guid id);
    Task<PermissionVM> CreatePermissionAsync(CreatePermissionRequest request);
    Task<PermissionVM> UpdatePermissionAsync(Guid id, UpdatePermissionRequest request);
    Task<bool> DeletePermissionAsync(Guid id);
    Task<bool> ActivatePermissionAsync(Guid id);
    Task<bool> DeactivatePermissionAsync(Guid id);

    // User Permissions
    Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId);
    Task<IEnumerable<PermissionVM>> GetUserPermissionDetailsAsync(Guid userId);
    Task<bool> HasPermissionAsync(Guid userId, string permission);
    Task<bool> HasAnyPermissionAsync(Guid userId, params string[] permissions);
    Task<bool> HasAllPermissionsAsync(Guid userId, params string[] permissions);
    Task<bool> GrantUserPermissionAsync(Guid userId, string permission, string? grantedBy = null, string? reason = null, DateTime? expiresAt = null);
    Task<bool> RevokeUserPermissionAsync(Guid userId, string permission, string? revokedBy = null, string? reason = null);
    Task<bool> GrantUserPermissionsAsync(Guid userId, IEnumerable<string> permissions, string? grantedBy = null, string? reason = null);
    Task<bool> RevokeUserPermissionsAsync(Guid userId, IEnumerable<string> permissions, string? revokedBy = null, string? reason = null);

    // Role Permissions
    Task<IEnumerable<string>> GetRolePermissionsAsync(string roleName);
    Task<IEnumerable<PermissionVM>> GetRolePermissionDetailsAsync(string roleName);
    Task<bool> RoleHasPermissionAsync(string roleName, string permission);
    Task<bool> GrantRolePermissionAsync(string roleName, string permission, string? grantedBy = null, string? reason = null, DateTime? expiresAt = null);
    Task<bool> RevokeRolePermissionAsync(string roleName, string permission, string? revokedBy = null, string? reason = null);
    Task<bool> GrantRolePermissionsAsync(string roleName, IEnumerable<string> permissions, string? grantedBy = null, string? reason = null);
    Task<bool> RevokeRolePermissionsAsync(string roleName, IEnumerable<string> permissions, string? revokedBy = null, string? reason = null);

    // Permission Audit
    Task<IEnumerable<PermissionAuditVM>> GetUserPermissionAuditAsync(Guid userId, int page = 1, int pageSize = 50);
    Task<IEnumerable<PermissionAuditVM>> GetRolePermissionAuditAsync(string roleName, int page = 1, int pageSize = 50);
    Task<IEnumerable<PermissionAuditVM>> GetPermissionAuditAsync(string permission, int page = 1, int pageSize = 50);

    // Bulk Operations
    Task<bool> SyncUserPermissionsAsync(Guid userId, IEnumerable<string> permissions, string? updatedBy = null);
    Task<bool> SyncRolePermissionsAsync(string roleName, IEnumerable<string> permissions, string? updatedBy = null);

    // System Operations
    Task<bool> InitializeSystemPermissionsAsync();
    Task<Dictionary<string, List<string>>> GetPermissionCategoriesAsync();
    Task<IEnumerable<UserPermissionSummaryVM>> GetUsersWithPermissionAsync(string permission);
    Task<IEnumerable<RolePermissionSummaryVM>> GetRolesWithPermissionAsync(string permission);
}