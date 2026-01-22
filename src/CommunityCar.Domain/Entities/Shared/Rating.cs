using System;
using CommunityCar.Domain.Base;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Domain.Entities.Shared;

public class Rating : BaseEntity
{
    public Guid EntityId { get; private set; }
    public EntityType EntityType { get; private set; }
    public Guid UserId { get; private set; }
    public int Value { get; private set; } // e.g. 1-5 stars
    public string? Review { get; private set; }

    public Rating(Guid entityId, EntityType entityType, Guid userId, int value, string? review = null)
    {
        EntityId = entityId;
        EntityType = entityType;
        UserId = userId;
        Value = value;
        Review = review;
    }

    private Rating() { }

    public void UpdateValue(int newValue, string? newReview)
    {
        Value = newValue;
        Review = newReview;
        Audit(UpdatedBy);
    }
}
