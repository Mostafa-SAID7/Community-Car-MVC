using CommunityCar.Application.Features.Account.DTOs.Authorization;

namespace CommunityCar.Application.Common.Interfaces.Services.Authorization;

/// <summary>
/// Service for managing roles
/// </summary>
public interface IRoleService
{
    // Role Management
    Task<IEnumerable<RoleDTO>> GetAllRolesAsync();
    Task<IEnumerable<RoleDTO>> GetActiveRolesAsync();
    Task<IEnumerable<RoleDTO>> GetRolesByCategoryAsync(string category);
    Task<RoleDTO?> GetRoleByNameAsync(string name);
    Task<RoleDTO?> GetRoleByIdAsync(Guid id);
    Task<RoleDTO> CreateRoleAsync(CreateRoleRequest request);
    Task<RoleDTO> UpdateRoleAsync(Guid id, UpdateRoleRequest request);
    Task<bool> DeleteRoleAsync(Guid id);
    Task<bool> ActivateRoleAsync(Guid id);
    Task<bool> DeactivateRoleAsync(Guid id);

    // User Role Assignment
    Task<IEnumerable<string>> GetUserRolesAsync(Guid userId);
    Task<IEnumerable<RoleDTO>> GetUserRoleDetailsAsync(Guid userId);
    Task<bool> IsInRoleAsync(Guid userId, string roleName);
    Task<bool> AssignRoleAsync(Guid userId, string roleName, string? assignedBy = null);
    Task<bool> RemoveRoleAsync(Guid userId, string roleName, string? removedBy = null);
    Task<bool> AssignRolesAsync(Guid userId, IEnumerable<string> roleNames, string? assignedBy = null);
    Task<bool> RemoveRolesAsync(Guid userId, IEnumerable<string> roleNames, string? removedBy = null);
    Task<bool> SyncUserRolesAsync(Guid userId, IEnumerable<string> roleNames, string? updatedBy = null);

    // Role Hierarchy and Priority
    Task<IEnumerable<RoleDTO>> GetRoleHierarchyAsync();
    Task<RoleDTO?> GetHighestPriorityRoleAsync(Guid userId);
    Task<bool> UpdateRolePriorityAsync(Guid roleId, int priority);

    // Role Statistics
    Task<RoleStatisticsDTO> GetRoleStatisticsAsync(string roleName);
    Task<IEnumerable<UserRoleSummaryDTO>> GetUsersInRoleAsync(string roleName, int page = 1, int pageSize = 50);
    Task<int> GetUserCountInRoleAsync(string roleName);

    // System Operations
    Task<bool> InitializeSystemRolesAsync();
    Task<Dictionary<string, List<RoleDTO>>> GetRoleCategoriesAsync();
    Task<bool> RoleExistsAsync(string roleName);
}