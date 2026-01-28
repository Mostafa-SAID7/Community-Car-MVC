using CommunityCar.Domain.Policies;
using CommunityCar.Domain.Policies.Account.Authorization;

namespace CommunityCar.Domain.Policies.Account.Core;

/// <summary>
/// Policy for specific admin operations
/// </summary>
public class AdminOperationPolicy : IAccessPolicy<AdminOperation>
{
    private readonly RequireAdminPolicy _adminPolicy;
    private readonly Func<Guid, Task<IEnumerable<string>>> _getUserPermissions;
    private readonly Dictionary<AdminOperation, string[]> _operationPermissions;

    public AdminOperationPolicy(
        RequireAdminPolicy adminPolicy,
        Func<Guid, Task<IEnumerable<string>>> getUserPermissions)
    {
        _adminPolicy = adminPolicy ?? throw new ArgumentNullException(nameof(adminPolicy));
        _getUserPermissions = getUserPermissions ?? throw new ArgumentNullException(nameof(getUserPermissions));
        
        _operationPermissions = new Dictionary<AdminOperation, string[]>
        {
            { AdminOperation.ViewUsers, new[] { "users.view", "admin.basic" } },
            { AdminOperation.EditUsers, new[] { "users.edit", "admin.moderate" } },
            { AdminOperation.DeleteUsers, new[] { "users.delete", "admin.advanced" } },
            { AdminOperation.ViewSystemLogs, new[] { "logs.view", "admin.basic" } },
            { AdminOperation.ManageRoles, new[] { "roles.manage", "admin.advanced" } },
            { AdminOperation.ManagePermissions, new[] { "permissions.manage", "admin.super" } },
            { AdminOperation.SystemConfiguration, new[] { "system.configure", "admin.super" } },
            { AdminOperation.DatabaseAccess, new[] { "database.access", "admin.super" } },
            { AdminOperation.SecuritySettings, new[] { "security.manage", "admin.super" } },
            { AdminOperation.UnlockAccounts, new[] { "accounts.unlock", "admin.moderate" } }
        };
    }

    public bool CanAccess(Guid userId, AdminOperation resource)
    {
        return CanAccessAsync(userId, resource).GetAwaiter().GetResult();
    }

    public async Task<bool> CanAccessAsync(Guid userId, AdminOperation operation)
    {
        // First check if user is admin
        if (!await _adminPolicy.CanAccessAsync(userId, new object()))
            return false;

        // Check specific operation permissions
        if (!_operationPermissions.TryGetValue(operation, out var requiredPermissions))
            return false;

        var userPermissions = await _getUserPermissions(userId);
        return requiredPermissions.Any(permission => 
            userPermissions.Contains(permission, StringComparer.OrdinalIgnoreCase));
    }
}