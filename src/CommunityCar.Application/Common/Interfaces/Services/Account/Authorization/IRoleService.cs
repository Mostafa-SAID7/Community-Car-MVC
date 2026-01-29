using CommunityCar.Application.Features.Account.ViewModels.Authorization;

namespace CommunityCar.Application.Common.Interfaces.Services.Account.Authorization;

/// <summary>
/// Service for managing roles
/// </summary>
public interface IRoleService
{
    // Role Management
    Task<IEnumerable<RoleVM>> GetAllRolesAsync();
    Task<IEnumerable<RoleVM>> GetActiveRolesAsync();
    Task<IEnumerable<RoleVM>> GetRolesByCategoryAsync(string category);
    Task<RoleVM?> GetRoleByNameAsync(string name);
    Task<RoleVM?> GetRoleByIdAsync(Guid id);
    Task<RoleVM> CreateRoleAsync(CreateRoleRequest request);
    Task<RoleVM> UpdateRoleAsync(Guid id, UpdateRoleRequest request);
    Task<bool> DeleteRoleAsync(Guid id);
    Task<bool> ActivateRoleAsync(Guid id);
    Task<bool> DeactivateRoleAsync(Guid id);

    // User Role Assignment
    Task<IEnumerable<string>> GetUserRolesAsync(Guid userId);
    Task<IEnumerable<RoleVM>> GetUserRoleDetailsAsync(Guid userId);
    Task<bool> IsInRoleAsync(Guid userId, string roleName);
    Task<bool> AssignRoleAsync(Guid userId, string roleName, string? assignedBy = null);
    Task<bool> RemoveRoleAsync(Guid userId, string roleName, string? removedBy = null);
    Task<bool> AssignRolesAsync(Guid userId, IEnumerable<string> roleNames, string? assignedBy = null);
    Task<bool> RemoveRolesAsync(Guid userId, IEnumerable<string> roleNames, string? removedBy = null);
    Task<bool> SyncUserRolesAsync(Guid userId, IEnumerable<string> roleNames, string? updatedBy = null);

    // Role Hierarchy and Priority
    Task<IEnumerable<RoleVM>> GetRoleHierarchyAsync();
    Task<RoleVM?> GetHighestPriorityRoleAsync(Guid userId);
    Task<bool> UpdateRolePriorityAsync(Guid roleId, int priority);

    // Role Statistics
    Task<RoleStatsVM> GetRoleStatisticsAsync(string roleName);
    Task<IEnumerable<UserRoleSummaryVM>> GetUsersInRoleAsync(string roleName, int page = 1, int pageSize = 50);
    Task<int> GetUserCountInRoleAsync(string roleName);

    // System Operations
    Task<bool> InitializeSystemRolesAsync();
    Task<Dictionary<string, List<RoleVM>>> GetRoleCategoriesAsync();
    Task<bool> RoleExistsAsync(string roleName);
}