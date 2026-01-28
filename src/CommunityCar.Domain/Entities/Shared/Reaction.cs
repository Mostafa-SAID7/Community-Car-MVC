using System;
using CommunityCar.Domain.Base;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Domain.Entities.Shared;

public class Reaction : BaseEntity
{
    public Guid EntityId { get; private set; }
    public EntityType EntityType { get; private set; }
    public Guid UserId { get; private set; }
    public ReactionType Type { get; private set; }

    public Reaction(Guid entityId, EntityType entityType, Guid userId, ReactionType type)
    {
        EntityId = entityId;
        EntityType = entityType;
        UserId = userId;
        Type = type;
    }

    private Reaction() { }

    public void UpdateType(ReactionType newType)
    {
        Type = newType;
        Audit(UpdatedBy);
    }
}
