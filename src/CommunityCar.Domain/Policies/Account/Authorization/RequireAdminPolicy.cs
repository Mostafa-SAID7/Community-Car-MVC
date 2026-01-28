using CommunityCar.Domain.Policies;

namespace CommunityCar.Domain.Policies.Account.Authorization;

/// <summary>
/// Policy that requires admin privileges for access
/// </summary>
public class RequireAdminPolicy : IAccessPolicy<object>
{
    private readonly Func<Guid, Task<bool>> _isAdminChecker;
    private readonly Func<Guid, Task<IEnumerable<string>>> _getUserRoles;

    public RequireAdminPolicy(
        Func<Guid, Task<bool>> isAdminChecker,
        Func<Guid, Task<IEnumerable<string>>> getUserRoles)
    {
        _isAdminChecker = isAdminChecker ?? throw new ArgumentNullException(nameof(isAdminChecker));
        _getUserRoles = getUserRoles ?? throw new ArgumentNullException(nameof(getUserRoles));
    }

    public bool CanAccess(Guid userId, object resource)
    {
        return CanAccessAsync(userId, resource).GetAwaiter().GetResult();
    }

    public async Task<bool> CanAccessAsync(Guid userId, object resource)
    {
        if (userId == Guid.Empty)
            return false;

        // First check direct admin status
        var isAdmin = await _isAdminChecker(userId);
        if (isAdmin)
            return true;

        // Check if user has admin roles
        var userRoles = await _getUserRoles(userId);
        var adminRoles = new[] { "Admin", "SuperAdmin", "SystemAdmin", "Moderator" };

        return userRoles.Any(role => adminRoles.Contains(role, StringComparer.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Checks if user has specific admin level
    /// </summary>
    public async Task<bool> HasAdminLevelAsync(Guid userId, AdminLevel requiredLevel)
    {
        var userRoles = await _getUserRoles(userId);
        
        return requiredLevel switch
        {
            AdminLevel.Basic => userRoles.Any(r => new[] { "Admin", "Moderator", "SuperAdmin", "SystemAdmin" }.Contains(r, StringComparer.OrdinalIgnoreCase)),
            AdminLevel.Moderate => userRoles.Any(r => new[] { "Admin", "SuperAdmin", "SystemAdmin" }.Contains(r, StringComparer.OrdinalIgnoreCase)),
            AdminLevel.Advanced => userRoles.Any(r => new[] { "SuperAdmin", "SystemAdmin" }.Contains(r, StringComparer.OrdinalIgnoreCase)),
            AdminLevel.Super => userRoles.Any(r => new[] { "SystemAdmin" }.Contains(r, StringComparer.OrdinalIgnoreCase)),
            _ => false
        };
    }

    /// <summary>
    /// Gets the highest admin level for a user
    /// </summary>
    public async Task<AdminLevel> GetUserAdminLevelAsync(Guid userId)
    {
        var userRoles = await _getUserRoles(userId);
        
        if (userRoles.Contains("SystemAdmin", StringComparer.OrdinalIgnoreCase))
            return AdminLevel.Super;
        if (userRoles.Contains("SuperAdmin", StringComparer.OrdinalIgnoreCase))
            return AdminLevel.Advanced;
        if (userRoles.Contains("Admin", StringComparer.OrdinalIgnoreCase))
            return AdminLevel.Moderate;
        if (userRoles.Contains("Moderator", StringComparer.OrdinalIgnoreCase))
            return AdminLevel.Basic;
            
        return AdminLevel.None;
    }
}

public enum AdminLevel
{
    None = 0,
    Basic = 1,      // Moderator
    Moderate = 2,   // Admin
    Advanced = 3,   // SuperAdmin
    Super = 4       // SystemAdmin
}