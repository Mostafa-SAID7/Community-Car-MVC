namespace CommunityCar.Application.Features.Account.DTOs.Authorization;

public class RoleDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsSystemRole { get; set; }
    public bool IsActive { get; set; }
    public int Priority { get; set; }
    public int UserCount { get; set; }
    public int PermissionCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public List<string> Permissions { get; set; } = new();
}

public class CreateRoleRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = "Custom";
    public int Priority { get; set; } = 0;
    public List<string> Permissions { get; set; } = new();
}

public class UpdateRoleRequest
{
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public int Priority { get; set; }
    public List<string> Permissions { get; set; } = new();
}

public class UserRoleDTO
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? RoleDescription { get; set; }
    public string RoleCategory { get; set; } = string.Empty;
    public int RolePriority { get; set; }
    public DateTime AssignedAt { get; set; }
    public string? AssignedBy { get; set; }
}

public class RoleStatisticsDTO
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int TotalPermissions { get; set; }
    public DateTime? LastAssigned { get; set; }
    public DateTime? LastRemoved { get; set; }
    public Dictionary<string, int> PermissionsByCategory { get; set; } = new();
    public List<UserRoleSummaryDTO> RecentAssignments { get; set; } = new();
}

public class UserRoleSummaryDTO
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public bool IsActive { get; set; }
    public DateTime AssignedAt { get; set; }
    public string? AssignedBy { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

public class AssignRoleRequest
{
    public Guid UserId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? Reason { get; set; }
}

public class RemoveRoleRequest
{
    public Guid UserId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? Reason { get; set; }
}

public class BulkRoleAssignmentRequest
{
    public Guid UserId { get; set; }
    public List<string> RoleNames { get; set; } = new();
    public string? Reason { get; set; }
}

public class RoleHierarchyDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public int Priority { get; set; }
    public bool IsSystemRole { get; set; }
    public List<RoleHierarchyDTO> SubRoles { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
}