using System;
using CommunityCar.Domain.Base;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Domain.Entities.Shared;

public class View : BaseEntity
{
    public Guid EntityId { get; private set; }
    public EntityType EntityType { get; private set; }
    public Guid? UserId { get; private set; } // Nullable for anonymous views
    public string IpAddress { get; private set; } = string.Empty;
    public string UserAgent { get; private set; } = string.Empty;

    // Parameterless constructor for EF
    private View() { }

    public View(Guid entityId, EntityType entityType, string ipAddress, string userAgent, Guid? userId = null)
    {
        EntityId = entityId;
        EntityType = entityType;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        UserId = userId;
    }
}
