using CommunityCar.Domain.Events;

namespace CommunityCar.Domain.Events.Account.Profile;

public class UserProfileUpdatedEvent : IDomainEvent
{
    public UserProfileUpdatedEvent(
        Guid userId,
        string email,
        DateTime updateDate,
        Dictionary<string, object> changedFields,
        Guid? updatedBy = null,
        string updateReason = null)
    {
        UserId = userId;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        UpdateDate = updateDate;
        ChangedFields = changedFields ?? throw new ArgumentNullException(nameof(changedFields));
        UpdatedBy = updatedBy;
        UpdateReason = updateReason;
        OccurredOn = DateTime.UtcNow;
    }

    public DateTime OccurredOn { get; }
    public Guid UserId { get; }
    public string Email { get; }
    public DateTime UpdateDate { get; }
    public Dictionary<string, object> ChangedFields { get; }
    public Guid? UpdatedBy { get; }
    public string UpdateReason { get; }
    public bool IsSelfUpdate => !UpdatedBy.HasValue || UpdatedBy.Value == UserId;
    public int FieldsChangedCount => ChangedFields.Count;
}
