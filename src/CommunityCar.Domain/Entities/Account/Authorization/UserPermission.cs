using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Account.Authorization;

/// <summary>
/// Direct user permissions (overrides role permissions)
/// </summary>
public class UserPermission : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid PermissionId { get; private set; }
    public bool IsGranted { get; private set; } = true;
    public DateTime? ExpiresAt { get; private set; }
    public string? GrantedBy { get; private set; }
    public string? Reason { get; private set; }
    public bool IsOverride { get; private set; } = false; // True if this overrides role permissions

    // Navigation properties
    public virtual Permission Permission { get; private set; } = null!;

    public UserPermission(Guid userId, Guid permissionId, bool isGranted = true, string? grantedBy = null, string? reason = null, DateTime? expiresAt = null, bool isOverride = false)
    {
        UserId = userId;
        PermissionId = permissionId;
        IsGranted = isGranted;
        GrantedBy = grantedBy;
        Reason = reason;
        ExpiresAt = expiresAt;
        IsOverride = isOverride;
    }

    // EF Core constructor
    private UserPermission() { }

    public void Grant(string? grantedBy = null, string? reason = null, bool isOverride = false)
    {
        IsGranted = true;
        GrantedBy = grantedBy;
        Reason = reason;
        IsOverride = isOverride;
        Audit(UpdatedBy);
    }

    public void Revoke(string? revokedBy = null, string? reason = null, bool isOverride = false)
    {
        IsGranted = false;
        Reason = reason;
        IsOverride = isOverride;
        Audit(revokedBy);
    }

    public void SetExpiration(DateTime? expiresAt)
    {
        ExpiresAt = expiresAt;
        Audit(UpdatedBy);
    }

    public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value <= DateTime.UtcNow;

    public bool IsEffective => !IsExpired;
}