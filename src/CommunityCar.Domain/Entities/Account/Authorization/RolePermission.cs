using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Account.Authorization;

/// <summary>
/// Junction table for Role-Permission many-to-many relationship
/// </summary>
public class RolePermission : BaseEntity
{
    public Guid RoleId { get; private set; }
    public Guid PermissionId { get; private set; }
    public bool IsGranted { get; private set; } = true;
    public DateTime? ExpiresAt { get; private set; }
    public string? GrantedBy { get; private set; }
    public string? Reason { get; private set; }

    // Navigation properties
    public virtual Role Role { get; private set; } = null!;
    public virtual Permission Permission { get; private set; } = null!;

    public RolePermission(Guid roleId, Guid permissionId, string? grantedBy = null, string? reason = null, DateTime? expiresAt = null)
    {
        RoleId = roleId;
        PermissionId = permissionId;
        IsGranted = true;
        GrantedBy = grantedBy;
        Reason = reason;
        ExpiresAt = expiresAt;
    }

    // EF Core constructor
    private RolePermission() { }

    public void Grant(string? grantedBy = null, string? reason = null)
    {
        IsGranted = true;
        GrantedBy = grantedBy;
        Reason = reason;
        Audit(UpdatedBy);
    }

    public void Revoke(string? revokedBy = null, string? reason = null)
    {
        IsGranted = false;
        Reason = reason;
        Audit(revokedBy);
    }

    public void SetExpiration(DateTime? expiresAt)
    {
        ExpiresAt = expiresAt;
        Audit(UpdatedBy);
    }

    public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value <= DateTime.UtcNow;

    public bool IsEffective => IsGranted && !IsExpired;
}