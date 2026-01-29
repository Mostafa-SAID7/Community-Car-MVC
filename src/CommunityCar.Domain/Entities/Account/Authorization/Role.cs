using Microsoft.AspNetCore.Identity;
using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Account.Authorization;

/// <summary>
/// Represents a role that can be assigned to users
/// Extends IdentityRole for ASP.NET Core Identity integration
/// </summary>
public class Role : IdentityRole<Guid>, IBaseEntity
{
    // BaseEntity properties
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    public bool IsDeleted { get; set; }

    public string? Description { get; private set; }
    public string Category { get; private set; } = string.Empty;
    public bool IsSystemRole { get; private set; }
    public bool IsActive { get; private set; } = true;
    public int Priority { get; private set; } = 0; // Higher number = higher priority

    // Navigation properties
    public virtual ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    public Role(string name, string? description = null, string category = "Custom", bool isSystemRole = false, int priority = 0)
    {
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        NormalizedName = name.ToUpperInvariant();
        Description = description;
        Category = category;
        IsSystemRole = isSystemRole;
        Priority = priority;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    // EF Core constructor
    public Role() 
    {
        Id = Guid.NewGuid();
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string? description, string category)
    {
        if (IsSystemRole)
            throw new InvalidOperationException("System roles cannot be modified");

        Description = description;
        Category = category ?? throw new ArgumentNullException(nameof(category));
        Audit(UpdatedBy);
    }

    public void UpdatePriority(int priority)
    {
        Priority = priority;
        Audit(UpdatedBy);
    }

    public void Deactivate()
    {
        if (IsSystemRole)
            throw new InvalidOperationException("System roles cannot be deactivated");

        IsActive = false;
        Audit(UpdatedBy);
    }

    public void Activate()
    {
        IsActive = true;
        Audit(UpdatedBy);
    }

    public void Audit(string? user)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = user;
    }
}