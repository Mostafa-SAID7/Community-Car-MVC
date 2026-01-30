using AutoMapper;
using CommunityCar.Application.Common.Interfaces.Repositories.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Account.Authorization;
using CommunityCar.Application.Features.Account.ViewModels.Authorization;
using CommunityCar.Domain.Constants;
using CommunityCar.Domain.Entities.Account.Authorization;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.Account.Authorization;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionService _permissionService;
    private readonly IMapper _mapper;
    private readonly ILogger<RoleService> _logger;

    public RoleService(
        IRoleRepository roleRepository,
        IPermissionService permissionService,
        IMapper mapper,
        ILogger<RoleService> logger)
    {
        _roleRepository = roleRepository;
        _permissionService = permissionService;
        _mapper = mapper;
        _logger = logger;
    }

    #region Role Management

    public async Task<IEnumerable<RoleVM>> GetAllRolesAsync()
    {
        var roles = await _roleRepository.GetAllAsync();
        var roleDTOs = new List<RoleVM>();

        foreach (var role in roles)
        {
            var roleDTO = _mapper.Map<RoleVM>(role);
            roleDTO.UserCount = await _roleRepository.GetUserCountInRoleAsync(role.Name!);
            roleDTO.Permissions = (await _permissionService.GetRolePermissionDetailsAsync(role.Name!)).ToList();
            roleDTO.PermissionCount = roleDTO.Permissions.Count;
            roleDTOs.Add(roleDTO);
        }

        return roleDTOs;
    }

    public async Task<IEnumerable<RoleVM>> GetActiveRolesAsync()
    {
        var roles = await _roleRepository.GetActiveAsync();
        return await MapRolesToDTOs(roles);
    }

    public async Task<IEnumerable<RoleVM>> GetRolesByCategoryAsync(string category)
    {
        var roles = await _roleRepository.GetByCategoryAsync(category);
        return await MapRolesToDTOs(roles);
    }

    public async Task<RoleVM?> GetRoleByNameAsync(string name)
    {
        var role = await _roleRepository.GetByNameAsync(name);
        if (role == null) return null;

        var roleDTO = _mapper.Map<RoleVM>(role);
        roleDTO.UserCount = await _roleRepository.GetUserCountInRoleAsync(name);
        roleDTO.Permissions = (await _permissionService.GetRolePermissionDetailsAsync(name)).ToList();
        roleDTO.PermissionCount = roleDTO.Permissions.Count;

        return roleDTO;
    }

    public async Task<RoleVM?> GetRoleByIdAsync(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null) return null;

        var roleDTO = _mapper.Map<RoleVM>(role);
        roleDTO.UserCount = await _roleRepository.GetUserCountInRoleAsync(role.Name!);
        roleDTO.Permissions = (await _permissionService.GetRolePermissionDetailsAsync(role.Name!)).ToList();
        roleDTO.PermissionCount = roleDTO.Permissions.Count;

        return roleDTO;
    }

    public async Task<RoleVM> CreateRoleAsync(CreateRoleRequest request)
    {
        if (await _roleRepository.ExistsAsync(request.Name))
            throw new InvalidOperationException($"Role '{request.Name}' already exists");

        var role = new Role(request.Name, request.Description, request.Category, false, request.Priority);
        var createdRole = await _roleRepository.AddAsync(role);

        // Assign permissions if provided
        if (request.Permissions.Any())
        {
            await _permissionService.GrantRolePermissionsAsync(request.Name, request.Permissions, "System", "Role creation");
        }

        _logger.LogInformation("Role '{RoleName}' created with {PermissionCount} permissions", request.Name, request.Permissions.Count);

        var roleDTO = _mapper.Map<RoleVM>(createdRole);
        roleDTO.Permissions = (await _permissionService.GetRolePermissionDetailsAsync(createdRole.Name!)).ToList();
        roleDTO.PermissionCount = roleDTO.Permissions.Count;
        roleDTO.UserCount = 0;

        return roleDTO;
    }

    public async Task<RoleVM> UpdateRoleAsync(Guid id, UpdateRoleRequest request)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null)
            throw new ArgumentException($"Role with ID '{id}' not found");

        role.UpdateDetails(request.Description, request.Category);
        role.UpdatePriority(request.Priority);
        
        var updatedRole = await _roleRepository.UpdateAsync(role);

        // Sync permissions
        if (request.Permissions.Any())
        {
            await _permissionService.SyncRolePermissionsAsync(role.Name!, request.Permissions, "System");
        }

        _logger.LogInformation("Role '{RoleName}' updated", role.Name);

        var roleDTO = _mapper.Map<RoleVM>(updatedRole);
        roleDTO.UserCount = await _roleRepository.GetUserCountInRoleAsync(role.Name!);
        roleDTO.Permissions = (await _permissionService.GetRolePermissionDetailsAsync(updatedRole.Name!)).ToList();
        roleDTO.PermissionCount = roleDTO.Permissions.Count;

        return roleDTO;
    }

    public async Task<bool> DeleteRoleAsync(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null) return false;

        if (role.IsSystemRole)
            throw new InvalidOperationException("System roles cannot be deleted");

        var userCount = await _roleRepository.GetUserCountInRoleAsync(role.Name!);
        if (userCount > 0)
            throw new InvalidOperationException($"Cannot delete role '{role.Name}' because it has {userCount} assigned users");

        var result = await _roleRepository.DeleteAsync(id);
        if (result)
            _logger.LogInformation("Role '{RoleName}' deleted", role.Name);

        return result;
    }

    public async Task<bool> ActivateRoleAsync(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null) return false;

        role.Activate();
        await _roleRepository.UpdateAsync(role);

        _logger.LogInformation("Role '{RoleName}' activated", role.Name);
        return true;
    }

    public async Task<bool> DeactivateRoleAsync(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null) return false;

        role.Deactivate();
        await _roleRepository.UpdateAsync(role);

        _logger.LogInformation("Role '{RoleName}' deactivated", role.Name);
        return true;
    }

    #endregion

    #region User Role Assignment

    public async Task<IEnumerable<string>> GetUserRolesAsync(Guid userId)
    {
        return await _roleRepository.GetUserRolesAsync(userId);
    }

    public async Task<IEnumerable<RoleVM>> GetUserRoleDetailsAsync(Guid userId)
    {
        var roles = await _roleRepository.GetUserRoleDetailsAsync(userId);
        return await MapRolesToDTOs(roles);
    }

    public async Task<bool> IsInRoleAsync(Guid userId, string roleName)
    {
        return await _roleRepository.IsUserInRoleAsync(userId, roleName);
    }

    public async Task<bool> AssignRoleAsync(Guid userId, string roleName, string? assignedBy = null)
    {
        if (!await _roleRepository.ExistsAsync(roleName))
            throw new ArgumentException($"Role '{roleName}' does not exist");

        var result = await _roleRepository.AddUserToRoleAsync(userId, roleName);
        if (result)
            _logger.LogInformation("Role '{RoleName}' assigned to user {UserId} by {AssignedBy}", roleName, userId, assignedBy ?? "System");

        return result;
    }

    public async Task<bool> RemoveRoleAsync(Guid userId, string roleName, string? removedBy = null)
    {
        var result = await _roleRepository.RemoveUserFromRoleAsync(userId, roleName);
        if (result)
            _logger.LogInformation("Role '{RoleName}' removed from user {UserId} by {RemovedBy}", roleName, userId, removedBy ?? "System");

        return result;
    }

    public async Task<bool> AssignRolesAsync(Guid userId, IEnumerable<string> roleNames, string? assignedBy = null)
    {
        var result = await _roleRepository.AddUserToRolesAsync(userId, roleNames);
        if (result)
            _logger.LogInformation("{RoleCount} roles assigned to user {UserId} by {AssignedBy}", roleNames.Count(), userId, assignedBy ?? "System");

        return result;
    }

    public async Task<bool> RemoveRolesAsync(Guid userId, IEnumerable<string> roleNames, string? removedBy = null)
    {
        var result = await _roleRepository.RemoveUserFromRolesAsync(userId, roleNames);
        if (result)
            _logger.LogInformation("{RoleCount} roles removed from user {UserId} by {RemovedBy}", roleNames.Count(), userId, removedBy ?? "System");

        return result;
    }

    public async Task<bool> SyncUserRolesAsync(Guid userId, IEnumerable<string> roleNames, string? updatedBy = null)
    {
        var result = await _roleRepository.SyncUserRolesAsync(userId, roleNames);
        if (result)
            _logger.LogInformation("User {UserId} roles synced to {RoleCount} roles by {UpdatedBy}", userId, roleNames.Count(), updatedBy ?? "System");

        return result;
    }

    #endregion

    #region Role Hierarchy and Priority

    public async Task<IEnumerable<RoleVM>> GetRoleHierarchyAsync()
    {
        var roles = await _roleRepository.GetRolesByPriorityAsync();
        return await MapRolesToDTOs(roles);
    }

    public async Task<RoleVM?> GetHighestPriorityRoleAsync(Guid userId)
    {
        var role = await _roleRepository.GetHighestPriorityUserRoleAsync(userId);
        if (role == null) return null;

        var roleDTO = _mapper.Map<RoleVM>(role);
        roleDTO.UserCount = await _roleRepository.GetUserCountInRoleAsync(role.Name!);
        roleDTO.Permissions = (await _permissionService.GetRolePermissionDetailsAsync(role.Name!)).ToList();
        roleDTO.PermissionCount = roleDTO.Permissions.Count;

        return roleDTO;
    }

    public async Task<bool> UpdateRolePriorityAsync(Guid roleId, int priority)
    {
        var result = await _roleRepository.UpdateRolePriorityAsync(roleId, priority);
        if (result)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            _logger.LogInformation("Role '{RoleName}' priority updated to {Priority}", role?.Name, priority);
        }

        return result;
    }

    #endregion

    #region Role Statistics

    public async Task<RoleStatsVM> GetRoleStatisticsAsync(string roleName)
    {
        var role = await _roleRepository.GetByNameAsync(roleName);
        if (role == null)
            throw new ArgumentException($"Role '{roleName}' not found");

        var userCount = await _roleRepository.GetUserCountInRoleAsync(roleName);
        var permissions = await _permissionService.GetRolePermissionDetailsAsync(roleName);
        var permissionsByCategory = permissions.GroupBy(p => p.Category)
            .ToDictionary(g => g.Key, g => g.Count());

        return new RoleStatsVM
        {
            RoleId = role.Id,
            RoleName = roleName,
            TotalUsers = userCount,
            ActiveUsers = userCount, // This would need additional logic to determine active users
            InactiveUsers = 0,
            TotalPermissions = permissions.Count(),
            PermissionsByCategory = permissionsByCategory,
            RecentAssignments = new List<UserRoleSummaryVM>() // This would need additional implementation
        };
    }

    public async Task<IEnumerable<UserRoleSummaryVM>> GetUsersInRoleAsync(string roleName, int page = 1, int pageSize = 50)
    {
        var userIds = await _roleRepository.GetUsersInRoleAsync(roleName);
        // This would need to be implemented with user repository to get user details
        return Enumerable.Empty<UserRoleSummaryVM>();
    }

    public async Task<int> GetUserCountInRoleAsync(string roleName)
    {
        return await _roleRepository.GetUserCountInRoleAsync(roleName);
    }

    #endregion

    #region System Operations

    public async Task<bool> InitializeSystemRolesAsync()
    {
        try
        {
            var systemRoles = GetSystemRoleDefinitions();

            foreach (var (roleName, roleInfo) in systemRoles)
            {
                var existingRole = await _roleRepository.GetByNameAsync(roleName);
                if (existingRole == null)
                {
                    var role = new Role(roleName, roleInfo.Description, roleInfo.Category, true, roleInfo.Priority);
                    await _roleRepository.AddAsync(role);

                    // Assign permissions
                    if (roleInfo.Permissions.Any())
                    {
                        await _permissionService.GrantRolePermissionsAsync(roleName, roleInfo.Permissions, "System", "System role initialization");
                    }

                    _logger.LogInformation("System role '{RoleName}' initialized with {PermissionCount} permissions", roleName, roleInfo.Permissions.Count);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize system roles");
            return false;
        }
    }

    public async Task<Dictionary<string, List<RoleVM>>> GetRoleCategoriesAsync()
    {
        var roles = await _roleRepository.GetAllAsync();
        var roleDTOs = await MapRolesToDTOs(roles);
        
        return roleDTOs
            .GroupBy(r => r.Category)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public async Task<bool> RoleExistsAsync(string roleName)
    {
        return await _roleRepository.ExistsAsync(roleName);
    }

    #endregion

    #region Private Methods

    private async Task<IEnumerable<RoleVM>> MapRolesToDTOs(IEnumerable<Role> roles)
    {
        var roleDTOs = new List<RoleVM>();

        foreach (var role in roles)
        {
            var roleDTO = _mapper.Map<RoleVM>(role);
            roleDTO.UserCount = await _roleRepository.GetUserCountInRoleAsync(role.Name!);
            roleDTO.Permissions = (await _permissionService.GetRolePermissionDetailsAsync(role.Name!)).ToList();
            roleDTO.PermissionCount = roleDTO.Permissions.Count;
            roleDTOs.Add(roleDTO);
        }

        return roleDTOs;
    }

    private static Dictionary<string, (string Description, string Category, int Priority, List<string> Permissions)> GetSystemRoleDefinitions()
    {
        return new Dictionary<string, (string, string, int, List<string>)>
        {
            [Roles.SuperAdmin] = (
                "Super Administrator with full system access",
                "Administration",
                1000,
                Permissions.GetAllPermissionsList()
            ),
            [Roles.Admin] = (
                "Administrator with broad system access",
                "Administration", 
                900,
                new List<string>
                {
                    Permissions.Users.View, Permissions.Users.Edit, Permissions.Users.ViewProfile,
                    Permissions.Content.View, Permissions.Content.Edit, Permissions.Content.Moderate,
                    Permissions.Community.ViewGroups, Permissions.Community.ManageGroups,
                    Permissions.System.ViewDashboard, Permissions.System.ViewMetrics,
                    Permissions.Analytics.ViewBasic, Permissions.Analytics.ViewAdvanced
                }
            ),
            [Roles.ContentAdmin] = (
                "Content Administrator",
                "Administration",
                800,
                new List<string>
                {
                    Permissions.Content.View, Permissions.Content.Create, Permissions.Content.Edit,
                    Permissions.Content.Delete, Permissions.Content.Publish, Permissions.Content.Moderate,
                    Permissions.Content.Feature, Permissions.Content.Verify,
                    Permissions.Media.View, Permissions.Media.Edit, Permissions.Media.Delete
                }
            ),
            [Roles.DatabaseAdmin] = (
                "Database Administrator",
                "Administration",
                850,
                new List<string>
                {
                    Permissions.System.DatabaseAccess, Permissions.System.BackupRestore,
                    Permissions.System.ViewLogs, Permissions.System.ViewMetrics
                }
            ),
            [Roles.DesignAdmin] = (
                "Design Administrator",
                "Administration",
                700,
                new List<string>
                {
                    Permissions.Content.View, Permissions.Content.Edit,
                    Permissions.Media.View, Permissions.Media.Upload, Permissions.Media.Edit,
                    Permissions.System.ViewDashboard
                }
            ),
            [Roles.Master] = (
                "Master User - Highest community level",
                "Community",
                500,
                new List<string>
                {
                    Permissions.Content.View, Permissions.Content.Create, Permissions.Content.Edit,
                    Permissions.Content.Publish, Permissions.Content.Feature,
                    Permissions.Community.ViewGroups, Permissions.Community.CreateGroups,
                    Permissions.Community.ViewEvents, Permissions.Community.CreateEvents,
                    Permissions.Media.View, Permissions.Media.Upload, Permissions.Media.Edit
                }
            ),
            [Roles.Author] = (
                "Content Author",
                "Community",
                400,
                new List<string>
                {
                    Permissions.Content.View, Permissions.Content.Create, Permissions.Content.Edit,
                    Permissions.Content.Publish,
                    Permissions.Media.View, Permissions.Media.Upload
                }
            ),
            [Roles.Reviewer] = (
                "Content Reviewer",
                "Community",
                300,
                new List<string>
                {
                    Permissions.Content.View, Permissions.Content.Create, Permissions.Content.Edit,
                    Permissions.Content.Moderate,
                    Permissions.Community.ModerateComments
                }
            ),
            [Roles.Expert] = (
                "Expert User",
                "Community",
                200,
                new List<string>
                {
                    Permissions.Content.View, Permissions.Content.Create, Permissions.Content.Edit,
                    Permissions.Community.ViewGroups, Permissions.Community.CreateGroups,
                    Permissions.Media.View, Permissions.Media.Upload
                }
            ),
            [Roles.User] = (
                "Regular User",
                "Community",
                100,
                new List<string>
                {
                    Permissions.Content.View, Permissions.Content.Create,
                    Permissions.Community.ViewGroups, Permissions.Community.ViewEvents,
                    Permissions.Media.View, Permissions.Media.Upload
                }
            )
        };
    }

    #endregion
}