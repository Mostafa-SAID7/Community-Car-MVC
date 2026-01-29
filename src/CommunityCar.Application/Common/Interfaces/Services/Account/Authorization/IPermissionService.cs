using CommunityCar.Application.Features.Account.DTOs.Authorization;

namespace CommunityCar.Application.Common.Interfaces.Services.Authorization;

/// <summary>
/// Service for managing permissions
/// </summary>
public interface IPermissionService
{
    // Permission Management
    Task<IEnumerable<PermissionDTO>> GetAllPermissionsAsync();
    Task<IEnumerable<PermissionDTO>> GetPermissionsByCategoryAsync(string category);
    Task<PermissionDTO?> GetPermissionByNameAsync(string name);
    Task<PermissionDTO?> GetPermissionByIdAsync(Guid id);
    Task<PermissionDTO> CreatePermissionAsync(CreatePermissionRequest request);
    Task<PermissionDTO> UpdatePermissionAsync(Guid id, UpdatePermissionRequest request);
    Task<bool> DeletePermissionAsync(Guid id);
    Task<bool> ActivatePermissionAsync(Guid id);
    Task<bool> DeactivatePermissionAsync(Guid id);

    // User Permissions
    Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId);
    Task<IEnumerable<PermissionDTO>> GetUserPermissionDetailsAsync(Guid userId);
    Task<bool> HasPermissionAsync(Guid userId, string permission);
    Task<bool> HasAnyPermissionAsync(Guid userId, params string[] permissions);
    Task<bool> HasAllPermissionsAsync(Guid userId, params string[] permissions);
    Task<bool> GrantUserPermissionAsync(Guid userId, string permission, string? grantedBy = null, string? reason = null, DateTime? expiresAt = null);
    Task<bool> RevokeUserPermissionAsync(Guid userId, string permission, string? revokedBy = null, string? reason = null);
    Task<bool> GrantUserPermissionsAsync(Guid userId, IEnumerable<string> permissions, string? grantedBy = null, string? reason = null);
    Task<bool> RevokeUserPermissionsAsync(Guid userId, IEnumerable<string> permissions, string? revokedBy = null, string? reason = null);

    // Role Permissions
    Task<IEnumerable<string>> GetRolePermissionsAsync(string roleName);
    Task<IEnumerable<PermissionDTO>> GetRolePermissionDetailsAsync(string roleName);
    Task<bool> RoleHasPermissionAsync(string roleName, string permission);
    Task<bool> GrantRolePermissionAsync(string roleName, string permission, string? grantedBy = null, string? reason = null, DateTime? expiresAt = null);
    Task<bool> RevokeRolePermissionAsync(string roleName, string permission, string? revokedBy = null, string? reason = null);
    Task<bool> GrantRolePermissionsAsync(string roleName, IEnumerable<string> permissions, string? grantedBy = null, string? reason = null);
    Task<bool> RevokeRolePermissionsAsync(string roleName, IEnumerable<string> permissions, string? revokedBy = null, string? reason = null);

    // Permission Audit
    Task<IEnumerable<PermissionAuditDTO>> GetUserPermissionAuditAsync(Guid userId, int page = 1, int pageSize = 50);
    Task<IEnumerable<PermissionAuditDTO>> GetRolePermissionAuditAsync(string roleName, int page = 1, int pageSize = 50);
    Task<IEnumerable<PermissionAuditDTO>> GetPermissionAuditAsync(string permission, int page = 1, int pageSize = 50);

    // Bulk Operations
    Task<bool> SyncUserPermissionsAsync(Guid userId, IEnumerable<string> permissions, string? updatedBy = null);
    Task<bool> SyncRolePermissionsAsync(string roleName, IEnumerable<string> permissions, string? updatedBy = null);

    // System Operations
    Task<bool> InitializeSystemPermissionsAsync();
    Task<Dictionary<string, List<string>>> GetPermissionCategoriesAsync();
    Task<IEnumerable<UserPermissionSummaryDTO>> GetUsersWithPermissionAsync(string permission);
    Task<IEnumerable<RolePermissionSummaryDTO>> GetRolesWithPermissionAsync(string permission);
}