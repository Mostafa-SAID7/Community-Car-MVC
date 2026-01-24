using System;
using System.Collections.Generic;

namespace CommunityCar.Domain.Base;

public abstract class BaseEntity : IBaseEntity, ISoftDeletable
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    public string? CreatedBy { get; private set; }
    public string? UpdatedBy { get; private set; }
    
    // Soft Delete Properties
    public bool IsDeleted { get; private set; } = false;
    public DateTime? DeletedAt { get; private set; }
    public string? DeletedBy { get; private set; }

    public void Audit(string user)
    {
        if (CreatedBy == null)
            CreatedBy = user;
        else
        {
            UpdatedBy = user;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public virtual void SoftDelete(string deletedBy)
    {
        if (IsDeleted) return;
        
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        UpdatedBy = deletedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public virtual void Restore(string restoredBy)
    {
        if (!IsDeleted) return;
        
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        UpdatedBy = restoredBy;
        UpdatedAt = DateTime.UtcNow;
    }
}
