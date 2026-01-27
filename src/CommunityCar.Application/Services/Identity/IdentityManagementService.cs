using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Services.Identity.User;
using CommunityCar.Application.Services.Identity.Role;
using CommunityCar.Application.Services.Identity.Claims;
using CommunityCar.Application.Features.Identity.ViewModels;
using CommunityCar.Application.Features.Identity.DTOs;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.Identity;

/// <summary>
/// Orchestrator service for identity management operations (users, roles, claims)
/// </summary>
public class IdentityManagementService : IIdentityManagementService
{
    private readonly IUserIdentityService _userIdentityService;
    private readonly IRoleManagementService _roleManagementService;
    private readonly IClaimsManagementService _claimsManagementService;
    private readonly ILogger<IdentityManagementService> _logger;

    public IdentityManagementService(
        IUserIdentityService userIdentityService,
        IRoleManagementService roleManagementService,
        IClaimsManagementService claimsManagementService,
        ILogger<IdentityManagementService> logger)
    {
        _userIdentityService = userIdentityService;
        _roleManagementService = roleManagementService;
        _claimsManagementService = claimsManagementService;
        _logger = logger;
    }

    #region User Identity - Delegate to UserIdentityService

    public async Task<UserIdentityVM?> GetUserIdentityAsync(Guid userId)
        => await _userIdentityService.GetUserIdentityAsync(userId);

    public async Task<IEnumerable<UserIdentityVM>> GetAllUsersAsync(int page = 1, int pageSize = 20)
        => await _userIdentityService.GetAllUsersAsync(page, pageSize);

    public async Task<bool> IsUserActiveAsync(Guid userId)
        => await _userIdentityService.IsUserActiveAsync(userId);

    public async Task<bool> LockUserAsync(Guid userId, string reason)
        => await _userIdentityService.LockUserAsync(userId, reason);

    public async Task<bool> UnlockUserAsync(Guid userId)
        => await _userIdentityService.UnlockUserAsync(userId);

    #endregion

    #region Role Management - Delegate to RoleManagementService

    public async Task<IEnumerable<RoleVM>> GetAllRolesAsync()
        => await _roleManagementService.GetAllRolesAsync();

    public async Task<bool> CreateRoleAsync(CreateRoleRequest request)
        => await _roleManagementService.CreateRoleAsync(request);

    public async Task<bool> UpdateRoleAsync(UpdateRoleRequest request)
        => await _roleManagementService.UpdateRoleAsync(request);

    public async Task<bool> DeleteRoleAsync(string roleName)
        => await _roleManagementService.DeleteRoleAsync(roleName);

    public async Task<bool> AssignRoleToUserAsync(Guid userId, string roleName)
        => await _roleManagementService.AssignRoleToUserAsync(userId, roleName);

    public async Task<bool> RemoveRoleFromUserAsync(Guid userId, string roleName)
        => await _roleManagementService.RemoveRoleFromUserAsync(userId, roleName);

    public async Task<IEnumerable<string>> GetUserRolesAsync(Guid userId)
        => await _roleManagementService.GetUserRolesAsync(userId);

    #endregion

    #region Claims Management - Delegate to ClaimsManagementService

    public async Task<IEnumerable<UserClaimVM>> GetUserClaimsAsync(Guid userId)
        => await _claimsManagementService.GetUserClaimsAsync(userId);

    public async Task<bool> AddClaimToUserAsync(Guid userId, string claimType, string claimValue)
        => await _claimsManagementService.AddClaimToUserAsync(userId, claimType, claimValue);

    public async Task<bool> RemoveClaimFromUserAsync(Guid userId, string claimType, string claimValue)
        => await _claimsManagementService.RemoveClaimFromUserAsync(userId, claimType, claimValue);

    public async Task<bool> UpdateUserClaimAsync(Guid userId, string oldClaimType, string oldClaimValue, string newClaimType, string newClaimValue)
        => await _claimsManagementService.UpdateUserClaimAsync(userId, oldClaimType, oldClaimValue, newClaimType, newClaimValue);

    #endregion
}


