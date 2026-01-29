using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Account.Authorization;

/// <summary>
/// Represents a permission that can be granted to roles or users
/// </summary>
public class Permission : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string Category { get; private set; } = string.Empty;
    public bool IsSystemPermission { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigation properties
    public virtual ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();
    public virtual ICollection<UserPermission> UserPermissions { get; private set; } = new List<UserPermission>();

    public Permission(string name, string displayName, string category, string? description = null, bool isSystemPermission = false)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        Category = category ?? throw new ArgumentNullException(nameof(category));
        Description = description;
        IsSystemPermission = isSystemPermission;
        IsActive = true;
    }

    // EF Core constructor
    private Permission() { }

    public void UpdateDetails(string displayName, string? description)
    {
        if (IsSystemPermission)
            throw new InvalidOperationException("System permissions cannot be modified");

        DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        Description = description;
        Audit(UpdatedBy);
    }

    public void Deactivate()
    {
        if (IsSystemPermission)
            throw new InvalidOperationException("System permissions cannot be deactivated");

        IsActive = false;
        Audit(UpdatedBy);
    }

    public void Activate()
    {
        IsActive = true;
        Audit(UpdatedBy);
    }
}