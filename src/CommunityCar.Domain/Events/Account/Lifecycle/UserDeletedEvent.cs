using CommunityCar.Domain.Events;

namespace CommunityCar.Domain.Events.Account.Lifecycle;

public class UserDeletedEvent : IDomainEvent
{
    public UserDeletedEvent(
        Guid userId,
        string email,
        string username,
        DateTime deletionDate,
        Guid? deletedBy = null,
        string deletionReason = null,
        bool isSoftDelete = true)
    {
        UserId = userId;
        Email = email;
        Username = username;
        DeletionDate = deletionDate;
        DeletedBy = deletedBy;
        DeletionReason = deletionReason;
        IsSoftDelete = isSoftDelete;
        OccurredOn = DateTime.UtcNow;
    }

    public DateTime OccurredOn { get; }
    public Guid UserId { get; }
    public string Email { get; }
    public string Username { get; }
    public DateTime DeletionDate { get; }
    public Guid? DeletedBy { get; }
    public string DeletionReason { get; }
    public bool IsSoftDelete { get; }
}