using AutoMapper;
using CommunityCar.Application.Common.Interfaces.Repositories.Authorization;
using CommunityCar.Web.Areas.Dashboard.Interfaces.Services.Management.Authorization;
using CommunityCar.Application.Features.Account.ViewModels.Authorization;
using CommunityCar.Domain.Constants;
using CommunityCar.Domain.Entities.Account.Authorization;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Web.Areas.Dashboard.Services.Management.Authorization;

public class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PermissionService> _logger;

    public PermissionService(
        IPermissionRepository permissionRepository,
        IRoleRepository roleRepository,
        IMapper mapper,
        ILogger<PermissionService> logger)
    {
        _permissionRepository = permissionRepository;
        _roleRepository = roleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    #region Permission Management

    public async Task<IEnumerable<PermissionVM>> GetAllPermissionsAsync()
    {
        var permissions = await _permissionRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<PermissionVM>>(permissions);
    }

    public async Task<IEnumerable<PermissionVM>> GetPermissionsByCategoryAsync(string category)
    {
        var permissions = await _permissionRepository.GetByCategoryAsync(category);
        return _mapper.Map<IEnumerable<PermissionVM>>(permissions);
    }

    public async Task<PermissionVM?> GetPermissionByNameAsync(string name)
    {
        var permission = await _permissionRepository.GetByNameAsync(name);
        return permission != null ? _mapper.Map<PermissionVM>(permission) : null;
    }

    public async Task<PermissionVM?> GetPermissionByIdAsync(Guid id)
    {
        var permission = await _permissionRepository.GetByIdAsync(id);
        return permission != null ? _mapper.Map<PermissionVM>(permission) : null;
    }

    public async Task<PermissionVM> CreatePermissionAsync(CreatePermissionRequest request)
    {
        if (await _permissionRepository.ExistsAsync(request.Name))
            throw new InvalidOperationException($"Permission '{request.Name}' already exists");

        var permission = new Permission(
            request.Name,
            request.DisplayName,
            request.Category,
            request.Description,
            request.IsSystemPermission);

        var createdPermission = await _permissionRepository.AddAsync(permission);
        _logger.LogInformation("Permission '{PermissionName}' created", request.Name);

        return _mapper.Map<PermissionVM>(createdPermission);
    }

    public async Task<PermissionVM> UpdatePermissionAsync(Guid id, UpdatePermissionRequest request)
    {
        var permission = await _permissionRepository.GetByIdAsync(id);
        if (permission == null)
            throw new ArgumentException($"Permission with ID '{id}' not found");

        permission.UpdateDetails(request.DisplayName, request.Description);
        var updatedPermission = await _permissionRepository.UpdateAsync(permission);

        _logger.LogInformation("Permission '{PermissionName}' updated", permission.Name);
        return _mapper.Map<PermissionVM>(updatedPermission);
    }

    public async Task<bool> DeletePermissionAsync(Guid id)
    {
        var permission = await _permissionRepository.GetByIdAsync(id);
        if (permission == null) return false;

        if (permission.IsSystemPermission)
            throw new InvalidOperationException("System permissions cannot be deleted");

        var result = await _permissionRepository.DeleteAsync(id);
        if (result)
            _logger.LogInformation("Permission '{PermissionName}' deleted", permission.Name);

        return result;
    }

    public async Task<bool> ActivatePermissionAsync(Guid id)
    {
        var permission = await _permissionRepository.GetByIdAsync(id);
        if (permission == null) return false;

        permission.Activate();
        await _permissionRepository.UpdateAsync(permission);

        _logger.LogInformation("Permission '{PermissionName}' activated", permission.Name);
        return true;
    }

    public async Task<bool> DeactivatePermissionAsync(Guid id)
    {
        var permission = await _permissionRepository.GetByIdAsync(id);
        if (permission == null) return false;

        permission.Deactivate();
        await _permissionRepository.UpdateAsync(permission);

        _logger.LogInformation("Permission '{PermissionName}' deactivated", permission.Name);
        return true;
    }

    #endregion

    #region User Permissions

    public async Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId)
    {
        return await _permissionRepository.GetEffectiveUserPermissionsAsync(userId);
    }

    public async Task<IEnumerable<PermissionVM>> GetUserPermissionDetailsAsync(Guid userId)
    {
        var userPermissions = await _permissionRepository.GetUserPermissionsAsync(userId);
        var rolePermissions = await GetUserRolePermissionsAsync(userId);

        var allPermissions = userPermissions.Select(up => new PermissionVM
        {
            Id = up.Permission.Id,
            Name = up.Permission.Name,
            DisplayName = up.Permission.DisplayName,
            Description = up.Permission.Description,
            Category = up.Permission.Category,
            IsSystemPermission = up.Permission.IsSystemPermission,
            IsActive = up.Permission.IsActive
        }).ToList();

        allPermissions.AddRange(rolePermissions);
        return allPermissions.DistinctBy(p => p.Name);
    }

    public async Task<bool> HasPermissionAsync(Guid userId, string permission)
    {
        return await _permissionRepository.UserHasPermissionAsync(userId, permission);
    }

    public async Task<bool> HasAnyPermissionAsync(Guid userId, params string[] permissions)
    {
        var userPermissions = await GetUserPermissionsAsync(userId);
        return permissions.Any(p => userPermissions.Contains(p));
    }

    public async Task<bool> HasAllPermissionsAsync(Guid userId, params string[] permissions)
    {
        var userPermissions = await GetUserPermissionsAsync(userId);
        return permissions.All(p => userPermissions.Contains(p));
    }

    public async Task<bool> GrantUserPermissionAsync(Guid userId, string permission, string? grantedBy = null, string? reason = null, DateTime? expiresAt = null)
    {
        var permissionEntity = await _permissionRepository.GetByNameAsync(permission);
        if (permissionEntity == null)
            throw new ArgumentException($"Permission '{permission}' not found");

        var existingPermission = await _permissionRepository.GetUserPermissionByNameAsync(userId, permission);
        if (existingPermission != null)
        {
            if (existingPermission.IsGranted && existingPermission.IsEffective)
                return true; // Already granted

            existingPermission.Grant(grantedBy, reason);
            existingPermission.SetExpiration(expiresAt);
            await _permissionRepository.UpdateUserPermissionAsync(existingPermission);
        }
        else
        {
            var userPermission = new UserPermission(userId, permissionEntity.Id, true, grantedBy, reason, expiresAt);
            await _permissionRepository.AddUserPermissionAsync(userPermission);
        }

        _logger.LogInformation("Permission '{Permission}' granted to user {UserId}", permission, userId);
        return true;
    }

    public async Task<bool> RevokeUserPermissionAsync(Guid userId, string permission, string? revokedBy = null, string? reason = null)
    {
        var existingPermission = await _permissionRepository.GetUserPermissionByNameAsync(userId, permission);
        if (existingPermission == null) return false;

        existingPermission.Revoke(revokedBy, reason);
        await _permissionRepository.UpdateUserPermissionAsync(existingPermission);

        _logger.LogInformation("Permission '{Permission}' revoked from user {UserId}", permission, userId);
        return true;
    }

    public async Task<bool> GrantUserPermissionsAsync(Guid userId, IEnumerable<string> permissions, string? grantedBy = null, string? reason = null)
    {
        var success = true;
        foreach (var permission in permissions)
        {
            success &= await GrantUserPermissionAsync(userId, permission, grantedBy, reason);
        }
        return success;
    }

    public async Task<bool> RevokeUserPermissionsAsync(Guid userId, IEnumerable<string> permissions, string? revokedBy = null, string? reason = null)
    {
        var success = true;
        foreach (var permission in permissions)
        {
            success &= await RevokeUserPermissionAsync(userId, permission, revokedBy, reason);
        }
        return success;
    }

    #endregion

    #region Role Permissions

    public async Task<IEnumerable<string>> GetRolePermissionsAsync(string roleName)
    {
        var role = await _roleRepository.GetByNameAsync(roleName);
        if (role == null) return Enumerable.Empty<string>();

        return await _permissionRepository.GetRolePermissionNamesAsync(role.Id);
    }

    public async Task<IEnumerable<PermissionVM>> GetRolePermissionDetailsAsync(string roleName)
    {
        var role = await _roleRepository.GetByNameAsync(roleName);
        if (role == null) return Enumerable.Empty<PermissionVM>();

        var rolePermissions = await _permissionRepository.GetRolePermissionsAsync(role.Id);
        return rolePermissions.Select(rp => _mapper.Map<PermissionVM>(rp.Permission));
    }

    public async Task<bool> RoleHasPermissionAsync(string roleName, string permission)
    {
        var role = await _roleRepository.GetByNameAsync(roleName);
        if (role == null) return false;

        return await _permissionRepository.RoleHasPermissionAsync(role.Id, permission);
    }

    public async Task<bool> GrantRolePermissionAsync(string roleName, string permission, string? grantedBy = null, string? reason = null, DateTime? expiresAt = null)
    {
        var role = await _roleRepository.GetByNameAsync(roleName);
        if (role == null)
            throw new ArgumentException($"Role '{roleName}' not found");

        var permissionEntity = await _permissionRepository.GetByNameAsync(permission);
        if (permissionEntity == null)
            throw new ArgumentException($"Permission '{permission}' not found");

        var existingPermission = await _permissionRepository.GetRolePermissionAsync(role.Id, permissionEntity.Id);
        if (existingPermission != null)
        {
            if (existingPermission.IsGranted && existingPermission.IsEffective)
                return true; // Already granted

            existingPermission.Grant(grantedBy, reason);
            existingPermission.SetExpiration(expiresAt);
            await _permissionRepository.UpdateRolePermissionAsync(existingPermission);
        }
        else
        {
            var rolePermission = new RolePermission(role.Id, permissionEntity.Id, grantedBy, reason, expiresAt);
            await _permissionRepository.AddRolePermissionAsync(rolePermission);
        }

        _logger.LogInformation("Permission '{Permission}' granted to role '{Role}'", permission, roleName);
        return true;
    }

    public async Task<bool> RevokeRolePermissionAsync(string roleName, string permission, string? revokedBy = null, string? reason = null)
    {
        var role = await _roleRepository.GetByNameAsync(roleName);
        if (role == null) return false;

        var existingPermission = await _permissionRepository.GetRolePermissionByNameAsync(roleName, permission);
        if (existingPermission == null) return false;

        existingPermission.Revoke(revokedBy, reason);
        await _permissionRepository.UpdateRolePermissionAsync(existingPermission);

        _logger.LogInformation("Permission '{Permission}' revoked from role '{Role}'", permission, roleName);
        return true;
    }

    public async Task<bool> GrantRolePermissionsAsync(string roleName, IEnumerable<string> permissions, string? grantedBy = null, string? reason = null)
    {
        var success = true;
        foreach (var permission in permissions)
        {
            success &= await GrantRolePermissionAsync(roleName, permission, grantedBy, reason);
        }
        return success;
    }

    public async Task<bool> RevokeRolePermissionsAsync(string roleName, IEnumerable<string> permissions, string? revokedBy = null, string? reason = null)
    {
        var success = true;
        foreach (var permission in permissions)
        {
            success &= await RevokeRolePermissionAsync(roleName, permission, revokedBy, reason);
        }
        return success;
    }

    #endregion

    #region System Operations

    public async Task<bool> InitializeSystemPermissionsAsync()
    {
        try
        {
            var allPermissions = Permissions.GetAllPermissions();
            
            foreach (var category in allPermissions)
            {
                foreach (var permissionName in category.Value)
                {
                    var existingPermission = await _permissionRepository.GetByNameAsync(permissionName);
                    if (existingPermission == null)
                    {
                        var displayName = FormatDisplayName(permissionName);
                        var permission = new Permission(permissionName, displayName, category.Key, null, true);
                        await _permissionRepository.AddAsync(permission);
                        _logger.LogInformation("System permission '{Permission}' initialized", permissionName);
                    }
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize system permissions");
            return false;
        }
    }

    public async Task<Dictionary<string, List<string>>> GetPermissionCategoriesAsync()
    {
        var permissions = await _permissionRepository.GetAllAsync();
        return permissions
            .GroupBy(p => p.Category)
            .ToDictionary(g => g.Key, g => g.Select(p => p.Name).ToList());
    }

    public async Task<IEnumerable<UserPermissionSummaryVM>> GetUsersWithPermissionAsync(string permission)
    {
        var userIds = await _permissionRepository.GetUsersWithPermissionAsync(permission);
        // This would need to be implemented with user repository to get user details
        return Enumerable.Empty<UserPermissionSummaryVM>();
    }

    public async Task<IEnumerable<RolePermissionSummaryVM>> GetRolesWithPermissionAsync(string permission)
    {
        var roleIds = await _permissionRepository.GetRolesWithPermissionAsync(permission);
        // This would need to be implemented with role repository to get role details
        return Enumerable.Empty<RolePermissionSummaryVM>();
    }

    #endregion

    #region Bulk Operations

    public async Task<bool> SyncUserPermissionsAsync(Guid userId, IEnumerable<string> permissions, string? updatedBy = null)
    {
        // Remove all existing permissions
        await _permissionRepository.RemoveAllUserPermissionsAsync(userId);

        // Add new permissions
        return await GrantUserPermissionsAsync(userId, permissions, updatedBy, "Bulk sync");
    }

    public async Task<bool> SyncRolePermissionsAsync(string roleName, IEnumerable<string> permissions, string? updatedBy = null)
    {
        var role = await _roleRepository.GetByNameAsync(roleName);
        if (role == null) return false;

        // Remove all existing permissions
        await _permissionRepository.RemoveAllRolePermissionsAsync(role.Id);

        // Add new permissions
        return await GrantRolePermissionsAsync(roleName, permissions, updatedBy, "Bulk sync");
    }

    #endregion

    #region Permission Audit

    public async Task<IEnumerable<PermissionAuditVM>> GetUserPermissionAuditAsync(Guid userId, int page = 1, int pageSize = 50)
    {
        // This would need to be implemented with an audit repository
        return Enumerable.Empty<PermissionAuditVM>();
    }

    public async Task<IEnumerable<PermissionAuditVM>> GetRolePermissionAuditAsync(string roleName, int page = 1, int pageSize = 50)
    {
        // This would need to be implemented with an audit repository
        return Enumerable.Empty<PermissionAuditVM>();
    }

    public async Task<IEnumerable<PermissionAuditVM>> GetPermissionAuditAsync(string permission, int page = 1, int pageSize = 50)
    {
        // This would need to be implemented with an audit repository
        return Enumerable.Empty<PermissionAuditVM>();
    }

    #endregion

    #region Private Methods

    private async Task<IEnumerable<PermissionVM>> GetUserRolePermissionsAsync(Guid userId)
    {
        var userRoles = await _roleRepository.GetUserRolesAsync(userId);
        var allRolePermissions = new List<PermissionVM>();

        foreach (var roleName in userRoles)
        {
            var rolePermissions = await GetRolePermissionDetailsAsync(roleName);
            allRolePermissions.AddRange(rolePermissions);
        }

        return allRolePermissions.DistinctBy(p => p.Name);
    }

    private static string FormatDisplayName(string permissionName)
    {
        var parts = permissionName.Split('.');
        if (parts.Length >= 2)
        {
            var category = parts[0];
            var action = parts[1];
            return $"{char.ToUpper(action[0])}{action[1..]} {char.ToUpper(category[0])}{category[1..]}";
        }
        return permissionName;
    }

    #endregion
}


