namespace CommunityCar.Application.Features.Account.ViewModels.Authorization;

#region View Models

public class RoleVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsSystemRole { get; set; }
    public int Priority { get; set; }
    public int UserCount { get; set; }
    public List<PermissionVM> Permissions { get; set; } = new();
    public int PermissionCount { get; set; }
}

public class PermissionVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsSystemPermission { get; set; }
    public bool IsActive { get; set; }
}

public class RoleStatsVM
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int TotalPermissions { get; set; }
    public Dictionary<string, int> PermissionsByCategory { get; set; } = new();
    public List<UserRoleSummaryVM> RecentAssignments { get; set; } = new();
}

public class UserRoleSummaryVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public DateTime AssignedAt { get; set; }
}

public class UserPermissionSummaryVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public DateTime GrantedAt { get; set; }
}

public class RolePermissionSummaryVM
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public DateTime GrantedAt { get; set; }
}

public class PermissionAuditVM
{
    public Guid Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Actor { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? Details { get; set; }
}

#endregion

#region Request Models

public class CreateRoleRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = "Custom";
    public int Priority { get; set; }
    public List<string> Permissions { get; set; } = new();
}

public class UpdateRoleRequest
{
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public int Priority { get; set; }
    public List<string> Permissions { get; set; } = new();
}

public class CreatePermissionRequest
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Category { get; set; } = "Custom";
    public string? Description { get; set; }
    public bool IsSystemPermission { get; set; }
}

public class UpdatePermissionRequest
{
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
}

public class AssignRoleRequest
{
    public Guid UserId { get; set; }
    public string RoleName { get; set; } = string.Empty;
}

#endregion
