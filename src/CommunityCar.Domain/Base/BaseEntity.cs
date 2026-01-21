using System;
using System.Collections.Generic;

namespace CommunityCar.Domain.Base;

public abstract class BaseEntity : IBaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    public string? CreatedBy { get; private set; }
    public string? UpdatedBy { get; private set; }

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
}
