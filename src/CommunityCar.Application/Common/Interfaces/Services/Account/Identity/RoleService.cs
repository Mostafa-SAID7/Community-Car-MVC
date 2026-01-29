using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Domain.Entities.Account.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Services.Account.Identity;

public class RoleService : IRoleService
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<RoleService> _logger;

    public RoleService(
        RoleManager<IdentityRole<Guid>> roleManager,
        UserManager<User> userManager,
        ILogger<RoleService> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IdentityResult> CreateRoleAsync(string roleName)
    {
        try
        {
            if (await RoleExistsAsync(roleName))
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Role '{roleName}' already exists." });
            }

            var role = new IdentityRole<Guid>(roleName)
            {
                Id = Guid.NewGuid()
            };

            var result = await _roleManager.CreateAsync(role);
            
            if (result.Succeeded)
            {
                _logger.LogInformation("Role '{RoleName}' created successfully", roleName);
            }
            else
            {
                _logger.LogWarning("Failed to create role '{RoleName}': {Errors}", 
                    roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role '{RoleName}'", roleName);
            throw;
        }
    }

    public async Task<IdentityResult> DeleteRoleAsync(string roleName)
    {
        try
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Role '{roleName}' not found." });
            }

            var result = await _roleManager.DeleteAsync(role);
            
            if (result.Succeeded)
            {
                _logger.LogInformation("Role '{RoleName}' deleted successfully", roleName);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting role '{RoleName}'", roleName);
            throw;
        }
    }

    public async Task<bool> RoleExistsAsync(string roleName)
    {
        return await _roleManager.RoleExistsAsync(roleName);
    }

    public async Task<IEnumerable<IdentityRole<Guid>>> GetAllRolesAsync()
    {
        return await _roleManager.Roles.ToListAsync();
    }

    public async Task<IdentityResult> AddUserToRoleAsync(Guid userId, string roleName)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            if (!await RoleExistsAsync(roleName))
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Role '{roleName}' does not exist." });
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);
            
            if (result.Succeeded)
            {
                _logger.LogInformation("User '{UserId}' added to role '{RoleName}'", userId, roleName);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user '{UserId}' to role '{RoleName}'", userId, roleName);
            throw;
        }
    }

    public async Task<IdentityResult> RemoveUserFromRoleAsync(Guid userId, string roleName)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            
            if (result.Succeeded)
            {
                _logger.LogInformation("User '{UserId}' removed from role '{RoleName}'", userId, roleName);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing user '{UserId}' from role '{RoleName}'", userId, roleName);
            throw;
        }
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(Guid userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return Enumerable.Empty<string>();
            }

            return await _userManager.GetRolesAsync(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting roles for user '{UserId}'", userId);
            throw;
        }
    }

    public async Task<bool> IsUserInRoleAsync(Guid userId, string roleName)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return false;
            }

            return await _userManager.IsInRoleAsync(user, roleName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user '{UserId}' is in role '{RoleName}'", userId, roleName);
            throw;
        }
    }
}