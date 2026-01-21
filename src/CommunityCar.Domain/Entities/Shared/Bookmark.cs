using System;
using CommunityCar.Domain.Base;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Domain.Entities.Shared;

public class Bookmark : BaseEntity
{
    public Guid EntityId { get; private set; }
    public EntityType EntityType { get; private set; }
    public Guid UserId { get; private set; }
    public string? Note { get; private set; }

    public Bookmark(Guid entityId, EntityType entityType, Guid userId, string? note = null)
    {
        EntityId = entityId;
        EntityType = entityType;
        UserId = userId;
        Note = note;
    }

    private Bookmark() { }
}
