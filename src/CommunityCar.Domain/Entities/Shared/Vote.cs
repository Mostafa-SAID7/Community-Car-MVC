using System;
using CommunityCar.Domain.Base;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Domain.Entities.Shared;

public class Vote : BaseEntity
{
    public Guid EntityId { get; private set; }
    public EntityType EntityType { get; private set; }
    public Guid UserId { get; private set; }
    public VoteType Type { get; private set; }

    public Vote(Guid entityId, EntityType entityType, Guid userId, VoteType type)
    {
        EntityId = entityId;
        EntityType = entityType;
        UserId = userId;
        Type = type;
    }

    private Vote() { }

    public void ChangeVote(VoteType newType)
    {
        Type = newType;
        Audit(UpdatedBy);
    }
}
