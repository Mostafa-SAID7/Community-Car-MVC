using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Features.Identity.ViewModels;
using CommunityCar.Application.Features.Identity.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.Identity.Role;

/// <summary>
/// Service for role management operations (CRUD, assignments)
/// </summary>
public class RoleManagementService : IRoleManagementService
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly UserManager<Domain.Entities.Auth.User> _userManager;
    private readonly ILogger<RoleManagementService> _logger;

    public RoleManagementService(
        RoleManager<IdentityRole<Guid>> roleManager,
        UserManager<Domain.Entities.Auth.User> userManager,
        ILogger<RoleManagementService> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IEnumerable<RoleVM>> GetAllRolesAsync()
    {
        try
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return roles.Select(r => new RoleVM
            {
                Id = r.Id,
                Name = r.Name ?? string.Empty,
                NormalizedName = r.NormalizedName ?? string.Empty,
                ConcurrencyStamp = r.ConcurrencyStamp ?? string.Empty
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all roles");
            return Enumerable.Empty<RoleVM>();
        }
    }

    public async Task<bool> CreateRoleAsync(CreateRoleRequest request)
    {
        try
        {
            if (await _roleManager.RoleExistsAsync(request.Name))
            {
                _logger.LogWarning("Role {RoleName} already exists", request.Name);
                return false;
            }

            var role = new IdentityRole<Guid>(request.Name)
            {
                Id = Guid.NewGuid()
            };

            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                _logger.LogInformation("Role {RoleName} created successfully", request.Name);
                return true;
            }

            _logger.LogWarning("Failed to create role {RoleName}: {Errors}", 
                request.Name, string.Join(", ", result.Errors.Select(e => e.Description)));
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role {RoleName}", request.Name);
            return false;
        }
    }

    public async Task<bool> UpdateRoleAsync(UpdateRoleRequest request)
    {
        try
        {
            var role = await _roleManager.FindByIdAsync(request.Id.ToString());
            if (role == null)
            {
                _logger.LogWarning("Role with ID {RoleId} not found", request.Id);
                return false;
            }

            role.Name = request.Name;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                _logger.LogInformation("Role {RoleId} updated successfully", request.Id);
                return true;
            }

            _logger.LogWarning("Failed to update role {RoleId}: {Errors}", 
                request.Id, string.Join(", ", result.Errors.Select(e => e.Description)));
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role {RoleId}", request.Id);
            return false;
        }
    }

    public async Task<bool> DeleteRoleAsync(string roleName)
    {
        try
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                _logger.LogWarning("Role {RoleName} not found", roleName);
                return false;
            }

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                _logger.LogInformation("Role {RoleName} deleted successfully", roleName);
                return true;
            }

            _logger.LogWarning("Failed to delete role {RoleName}: {Errors}", 
                roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting role {RoleName}", roleName);
            return false;
        }
    }

    public async Task<bool> AssignRoleToUserAsync(Guid userId, string roleName)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found", userId);
                return false;
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                _logger.LogWarning("Role {RoleName} does not exist", roleName);
                return false;
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                _logger.LogInformation("Role {RoleName} assigned to user {UserId}", roleName, userId);
                return true;
            }

            _logger.LogWarning("Failed to assign role {RoleName} to user {UserId}: {Errors}", 
                roleName, userId, string.Join(", ", result.Errors.Select(e => e.Description)));
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role {RoleName} to user {UserId}", roleName, userId);
            return false;
        }
    }

    public async Task<bool> RemoveRoleFromUserAsync(Guid userId, string roleName)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found", userId);
                return false;
            }

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                _logger.LogInformation("Role {RoleName} removed from user {UserId}", roleName, userId);
                return true;
            }

            _logger.LogWarning("Failed to remove role {RoleName} from user {UserId}: {Errors}", 
                roleName, userId, string.Join(", ", result.Errors.Select(e => e.Description)));
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role {RoleName} from user {UserId}", roleName, userId);
            return false;
        }
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(Guid userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found", userId);
                return Enumerable.Empty<string>();
            }

            return await _userManager.GetRolesAsync(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting roles for user {UserId}", userId);
            return Enumerable.Empty<string>();
        }
    }
}


