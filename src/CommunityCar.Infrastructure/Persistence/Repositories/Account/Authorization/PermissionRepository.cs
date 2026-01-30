using CommunityCar.Application.Common.Interfaces.Repositories.Authorization;
using CommunityCar.Domain.Entities.Account.Authorization;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Account.Authorization;

public class PermissionRepository : BaseRepository<Permission>, IPermissionRepository
{
    public PermissionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Permission?> GetByNameAsync(string name)
    {
        return await DbSet.FirstOrDefaultAsync(p => p.Name == name);
    }

    public async Task<IEnumerable<Permission>> GetByCategoryAsync(string category)
    {
        return await DbSet.Where(p => p.Category == category).ToListAsync();
    }

    public async Task<IEnumerable<Permission>> GetActiveAsync()
    {
        return await DbSet.Where(p => p.IsActive).ToListAsync();
    }

    public new async Task<Permission> AddAsync(Permission permission)
    {
        await DbSet.AddAsync(permission);
        await Context.SaveChangesAsync();
        return permission;
    }

    public new async Task<Permission> UpdateAsync(Permission permission)
    {
        DbSet.Update(permission);
        await Context.SaveChangesAsync();
        return permission;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var permission = await GetByIdAsync(id);
        if (permission == null) return false;
        DbSet.Remove(permission);
        await Context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(string name)
    {
        return await DbSet.AnyAsync(p => p.Name == name);
    }

    public async Task<IEnumerable<UserPermission>> GetUserPermissionsAsync(Guid userId)
    {
        return await Context.Set<UserPermission>()
            .Include(up => up.Permission)
            .Where(up => up.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetUserPermissionNamesAsync(Guid userId)
    {
        return await Context.Set<UserPermission>()
            .Where(up => up.UserId == userId && up.IsGranted)
            .Select(up => up.Permission.Name)
            .ToListAsync();
    }

    public async Task<UserPermission?> GetUserPermissionAsync(Guid userId, Guid permissionId)
    {
        return await Context.Set<UserPermission>()
            .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId);
    }

    public async Task<UserPermission?> GetUserPermissionByNameAsync(Guid userId, string permissionName)
    {
        return await Context.Set<UserPermission>()
            .Include(up => up.Permission)
            .FirstOrDefaultAsync(up => up.UserId == userId && up.Permission.Name == permissionName);
    }

    public async Task<UserPermission> AddUserPermissionAsync(UserPermission userPermission)
    {
        await Context.Set<UserPermission>().AddAsync(userPermission);
        await Context.SaveChangesAsync();
        return userPermission;
    }

    public async Task<UserPermission> UpdateUserPermissionAsync(UserPermission userPermission)
    {
        Context.Set<UserPermission>().Update(userPermission);
        await Context.SaveChangesAsync();
        return userPermission;
    }

    public async Task<bool> RemoveUserPermissionAsync(Guid userId, Guid permissionId)
    {
        var userPermission = await GetUserPermissionAsync(userId, permissionId);
        if (userPermission == null) return false;
        Context.Set<UserPermission>().Remove(userPermission);
        await Context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveAllUserPermissionsAsync(Guid userId)
    {
        var userPermissions = await Context.Set<UserPermission>().Where(up => up.UserId == userId).ToListAsync();
        Context.Set<UserPermission>().RemoveRange(userPermissions);
        await Context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<RolePermission>> GetRolePermissionsAsync(Guid roleId)
    {
        return await Context.Set<RolePermission>()
            .Include(rp => rp.Permission)
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetRolePermissionNamesAsync(Guid roleId)
    {
        return await Context.Set<RolePermission>()
            .Where(rp => rp.RoleId == roleId && rp.IsGranted)
            .Select(rp => rp.Permission.Name)
            .ToListAsync();
    }

    public async Task<RolePermission?> GetRolePermissionAsync(Guid roleId, Guid permissionId)
    {
        return await Context.Set<RolePermission>()
            .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
    }

    public async Task<RolePermission?> GetRolePermissionByNameAsync(string roleName, string permissionName)
    {
        return await Context.Set<RolePermission>()
            .Include(rp => rp.Permission)
            .Include(rp => rp.Role)
            .FirstOrDefaultAsync(rp => rp.Role.Name == roleName && rp.Permission.Name == permissionName);
    }

    public async Task<RolePermission> AddRolePermissionAsync(RolePermission rolePermission)
    {
        await Context.Set<RolePermission>().AddAsync(rolePermission);
        await Context.SaveChangesAsync();
        return rolePermission;
    }

    public async Task<RolePermission> UpdateRolePermissionAsync(RolePermission rolePermission)
    {
        Context.Set<RolePermission>().Update(rolePermission);
        await Context.SaveChangesAsync();
        return rolePermission;
    }

    public async Task<bool> RemoveRolePermissionAsync(Guid roleId, Guid permissionId)
    {
        var rolePermission = await GetRolePermissionAsync(roleId, permissionId);
        if (rolePermission == null) return false;
        Context.Set<RolePermission>().Remove(rolePermission);
        await Context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveAllRolePermissionsAsync(Guid roleId)
    {
        var rolePermissions = await Context.Set<RolePermission>().Where(rp => rp.RoleId == roleId).ToListAsync();
        Context.Set<RolePermission>().RemoveRange(rolePermissions);
        await Context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UserHasPermissionAsync(Guid userId, string permissionName)
    {
        // Check direct permission first
        var directPermission = await GetUserPermissionByNameAsync(userId, permissionName);
        if (directPermission != null && directPermission.IsOverride)
        {
             return directPermission.IsGranted && directPermission.IsEffective;
        }

        // Check role permissions
        var userRoles = await Context.UserRoles.Where(ur => ur.UserId == userId).Select(ur => ur.RoleId).ToListAsync();
        var hasRolePermission = await Context.Set<RolePermission>()
            .Include(rp => rp.Permission)
            .AnyAsync(rp => userRoles.Contains(rp.RoleId) && rp.Permission.Name == permissionName && rp.IsEffective);

        if (hasRolePermission) return true;

        // If no role permission, check if direct permission (if exists but not override) grants it
        if (directPermission != null)
        {
            return directPermission.IsGranted && directPermission.IsEffective;
        }

        return false;
    }

    public async Task<bool> RoleHasPermissionAsync(Guid roleId, string permissionName)
    {
        return await Context.Set<RolePermission>()
            .Include(rp => rp.Permission)
            .AnyAsync(rp => rp.RoleId == roleId && rp.Permission.Name == permissionName && rp.IsEffective);
    }

    public async Task<IEnumerable<Guid>> GetUsersWithPermissionAsync(string permissionName)
    {
        // This is complex as it involves calculating effective permissions for all users
        // For simplicity, we'll return users with direct grants or grants through roles
        var usersWithDirectPermission = await Context.Set<UserPermission>()
            .Include(up => up.Permission)
            .Where(up => up.Permission.Name == permissionName && up.IsGranted && up.IsEffective)
            .Select(up => up.UserId)
            .ToListAsync();

        var rolesWithPermission = await Context.Set<RolePermission>()
            .Include(rp => rp.Permission)
            .Where(rp => rp.Permission.Name == permissionName && rp.IsEffective)
            .Select(rp => rp.RoleId)
            .ToListAsync();

        var usersInRoles = await Context.UserRoles
            .Where(ur => rolesWithPermission.Contains(ur.RoleId))
            .Select(ur => ur.UserId)
            .ToListAsync();

        return usersWithDirectPermission.Union(usersInRoles).ToList();
    }

    public async Task<IEnumerable<Guid>> GetRolesWithPermissionAsync(string permissionName)
    {
        return await Context.Set<RolePermission>()
            .Include(rp => rp.Permission)
            .Where(rp => rp.Permission.Name == permissionName && rp.IsEffective)
            .Select(rp => rp.RoleId)
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetEffectiveUserPermissionsAsync(Guid userId)
    {
        var userPermissions = await GetUserPermissionsAsync(userId);
        var directGrants = userPermissions.Where(up => up.IsGranted && up.IsEffective).Select(up => up.Permission.Name).ToList();
        var directRevokes = userPermissions.Where(up => !up.IsGranted && up.IsOverride).Select(up => up.Permission.Name).ToList();
        
        var userRoles = await Context.UserRoles.Where(ur => ur.UserId == userId).Select(ur => ur.RoleId).ToListAsync();
        var rolePermissions = await Context.Set<RolePermission>()
            .Include(rp => rp.Permission)
            .Where(rp => userRoles.Contains(rp.RoleId) && rp.IsEffective)
            .Select(rp => rp.Permission.Name)
            .ToListAsync();

        return rolePermissions.Except(directRevokes).Union(directGrants).Distinct().ToList();
    }

    public async Task<bool> BulkAddUserPermissionsAsync(Guid userId, IEnumerable<UserPermission> permissions)
    {
        await Context.Set<UserPermission>().AddRangeAsync(permissions);
        await Context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> BulkRemoveUserPermissionsAsync(Guid userId, IEnumerable<string> permissionNames)
    {
        var permissionsToRemove = await Context.Set<UserPermission>()
            .Include(up => up.Permission)
            .Where(up => up.UserId == userId && permissionNames.Contains(up.Permission.Name))
            .ToListAsync();
        
        Context.Set<UserPermission>().RemoveRange(permissionsToRemove);
        await Context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> BulkAddRolePermissionsAsync(Guid roleId, IEnumerable<RolePermission> permissions)
    {
        await Context.Set<RolePermission>().AddRangeAsync(permissions);
        await Context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> BulkRemoveRolePermissionsAsync(Guid roleId, IEnumerable<string> permissionNames)
    {
        var permissionsToRemove = await Context.Set<RolePermission>()
            .Include(rp => rp.Permission)
            .Where(rp => rp.RoleId == roleId && permissionNames.Contains(rp.Permission.Name))
            .ToListAsync();
        
        Context.Set<RolePermission>().RemoveRange(permissionsToRemove);
        await Context.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetPermissionCountAsync()
    {
        return await DbSet.CountAsync();
    }

    public async Task<int> GetUserPermissionCountAsync(Guid userId)
    {
        return await Context.Set<UserPermission>().CountAsync(up => up.UserId == userId);
    }

    public async Task<int> GetRolePermissionCountAsync(Guid roleId)
    {
        return await Context.Set<RolePermission>().CountAsync(rp => rp.RoleId == roleId);
    }

    public async Task<Dictionary<string, int>> GetPermissionCountByCategoryAsync()
    {
        return await DbSet.GroupBy(p => p.Category)
            .ToDictionaryAsync(g => g.Key, g => g.Count());
    }
}
