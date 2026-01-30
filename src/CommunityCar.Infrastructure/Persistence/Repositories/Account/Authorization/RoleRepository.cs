using CommunityCar.Application.Common.Interfaces.Repositories.Authorization;
using CommunityCar.Domain.Entities.Account.Authorization;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CommunityCar.Domain.Entities.Account.Core;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Account.Authorization;

public class RoleRepository : BaseRepository<Role>, IRoleRepository
{
    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<User> _userManager;

    public RoleRepository(
        ApplicationDbContext context, 
        RoleManager<Role> roleManager,
        UserManager<User> userManager) : base(context)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _roleManager.FindByNameAsync(name);
    }

    public async Task<IEnumerable<Role>> GetByCategoryAsync(string category)
    {
        return await DbSet.Where(r => r.Category == category).ToListAsync();
    }

    public async Task<IEnumerable<Role>> GetActiveAsync()
    {
        return await DbSet.Where(r => r.IsActive).ToListAsync();
    }

    public async Task<IEnumerable<Role>> GetSystemRolesAsync()
    {
        return await DbSet.Where(r => r.IsSystemRole).ToListAsync();
    }

    public new async Task<Role> AddAsync(Role role)
    {
        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            throw new Exception($"Failed to create role: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
        return role;
    }

    public new async Task<Role> UpdateAsync(Role role)
    {
        var result = await _roleManager.UpdateAsync(role);
        if (!result.Succeeded)
        {
            throw new Exception($"Failed to update role: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
        return role;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var role = await GetByIdAsync(id);
        if (role == null) return false;
        var result = await _roleManager.DeleteAsync(role);
        return result.Succeeded;
    }

    public async Task<bool> ExistsAsync(string name)
    {
        return await _roleManager.RoleExistsAsync(name);
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return Enumerable.Empty<string>();
        return await _userManager.GetRolesAsync(user);
    }

    public async Task<IEnumerable<Role>> GetUserRoleDetailsAsync(Guid userId)
    {
        var roleNames = await GetUserRolesAsync(userId);
        return await DbSet.Where(r => roleNames.Contains(r.Name!)).ToListAsync();
    }

    public async Task<bool> IsUserInRoleAsync(Guid userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;
        return await _userManager.IsInRoleAsync(user, roleName);
    }

    public async Task<bool> AddUserToRoleAsync(Guid userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;
        var result = await _userManager.AddToRoleAsync(user, roleName);
        return result.Succeeded;
    }

    public async Task<bool> RemoveUserFromRoleAsync(Guid userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;
        var result = await _userManager.RemoveFromRoleAsync(user, roleName);
        return result.Succeeded;
    }

    public async Task<bool> AddUserToRolesAsync(Guid userId, IEnumerable<string> roleNames)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;
        var result = await _userManager.AddToRolesAsync(user, roleNames);
        return result.Succeeded;
    }

    public async Task<bool> RemoveUserFromRolesAsync(Guid userId, IEnumerable<string> roleNames)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;
        var result = await _userManager.RemoveFromRolesAsync(user, roleNames);
        return result.Succeeded;
    }

    public async Task<IEnumerable<Guid>> GetUsersInRoleAsync(string roleName)
    {
        var users = await _userManager.GetUsersInRoleAsync(roleName);
        return users.Select(u => u.Id);
    }

    public async Task<IEnumerable<Role>> GetRolesByPriorityAsync()
    {
        return await DbSet.OrderByDescending(r => r.Priority).ToListAsync();
    }

    public async Task<Role?> GetHighestPriorityUserRoleAsync(Guid userId)
    {
        var roleNames = await GetUserRolesAsync(userId);
        if (!roleNames.Any()) return null;
        
        return await DbSet
            .Where(r => roleNames.Contains(r.Name!))
            .OrderByDescending(r => r.Priority)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateRolePriorityAsync(Guid roleId, int priority)
    {
        var role = await GetByIdAsync(roleId);
        if (role == null) return false;
        role.UpdatePriority(priority);
        await UpdateAsync(role);
        return true;
    }

    public async Task<int> GetRoleCountAsync()
    {
        return await DbSet.CountAsync();
    }

    public async Task<int> GetUserCountInRoleAsync(string roleName)
    {
        var role = await GetByNameAsync(roleName);
        if (role == null) return 0;
        
        return await Context.UserRoles.CountAsync(ur => ur.RoleId == role.Id);
    }

    public async Task<Dictionary<string, int>> GetRoleCountByCategoryAsync()
    {
        return await DbSet.GroupBy(r => r.Category)
            .ToDictionaryAsync(g => g.Key, g => g.Count());
    }

    public async Task<Dictionary<string, int>> GetUserCountByRoleAsync()
    {
        var roles = await GetAllAsync();
        var counts = new Dictionary<string, int>();
        foreach (var role in roles)
        {
            counts[role.Name!] = await GetUserCountInRoleAsync(role.Name!);
        }
        return counts;
    }

    public async Task<bool> BulkAssignRolesAsync(Guid userId, IEnumerable<string> roleNames)
    {
        return await AddUserToRolesAsync(userId, roleNames);
    }

    public async Task<bool> BulkRemoveRolesAsync(Guid userId, IEnumerable<string> roleNames)
    {
        return await RemoveUserFromRolesAsync(userId, roleNames);
    }

    public async Task<bool> SyncUserRolesAsync(Guid userId, IEnumerable<string> roleNames)
    {
        var currentRoles = await GetUserRolesAsync(userId);
        var rolesToRemove = currentRoles.Except(roleNames).ToList();
        var rolesToAdd = roleNames.Except(currentRoles).ToList();

        if (rolesToRemove.Any()) await RemoveUserFromRolesAsync(userId, rolesToRemove);
        if (rolesToAdd.Any()) await AddUserToRolesAsync(userId, rolesToAdd);

        return true;
    }
}
