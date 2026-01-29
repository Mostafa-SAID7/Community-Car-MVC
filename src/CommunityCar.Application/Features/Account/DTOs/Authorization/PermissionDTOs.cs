namespace CommunityCar.Application.Features.Account.DTOs.Authorization;

public class PermissionDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsSystemPermission { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

public class CreatePermissionRequest
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsSystemPermission { get; set; } = false;
}

public class UpdatePermissionRequest
{
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
}

public class UserPermissionDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid PermissionId { get; set; }
    public string PermissionName { get; set; } = string.Empty;
    public string PermissionDisplayName { get; set; } = string.Empty;
    public string PermissionCategory { get; set; } = string.Empty;
    public bool IsGranted { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? GrantedBy { get; set; }
    public string? Reason { get; set; }
    public bool IsOverride { get; set; }
    public bool IsExpired { get; set; }
    public bool IsEffective { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class RolePermissionDTO
{
    public Guid Id { get; set; }
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public Guid PermissionId { get; set; }
    public string PermissionName { get; set; } = string.Empty;
    public string PermissionDisplayName { get; set; } = string.Empty;
    public string PermissionCategory { get; set; } = string.Empty;
    public bool IsGranted { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? GrantedBy { get; set; }
    public string? Reason { get; set; }
    public bool IsExpired { get; set; }
    public bool IsEffective { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class PermissionAuditDTO
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public Guid? RoleId { get; set; }
    public string? RoleName { get; set; }
    public string PermissionName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // Grant, Revoke, Expire
    public string? PerformedBy { get; set; }
    public string? Reason { get; set; }
    public DateTime PerformedAt { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}

public class UserPermissionSummaryDTO
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public bool IsDirectPermission { get; set; }
    public List<string> GrantedThroughRoles { get; set; } = new();
    public DateTime? ExpiresAt { get; set; }
    public bool IsExpired { get; set; }
}

public class RolePermissionSummaryDTO
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public int UserCount { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsExpired { get; set; }
}

public class GrantPermissionRequest
{
    public Guid UserId { get; set; }
    public string Permission { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsOverride { get; set; } = false;
}

public class RevokePermissionRequest
{
    public Guid UserId { get; set; }
    public string Permission { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public bool IsOverride { get; set; } = false;
}

public class BulkPermissionRequest
{
    public Guid UserId { get; set; }
    public List<string> Permissions { get; set; } = new();
    public string? Reason { get; set; }
    public DateTime? ExpiresAt { get; set; }
}