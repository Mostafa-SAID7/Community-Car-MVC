using CommunityCar.Domain.Events;

namespace CommunityCar.Domain.Events.Account.Profile;

public class UserRolesChangedEvent : IDomainEvent
{
    public UserRolesChangedEvent(
        Guid userId,
        string email,
        DateTime changeDate,
        IEnumerable<string> oldRoles,
        IEnumerable<string> newRoles,
        Guid changedBy,
        string changeReason = null)
    {
        UserId = userId;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        ChangeDate = changeDate;
        OldRoles = oldRoles?.ToList() ?? new List<string>();
        NewRoles = newRoles?.ToList() ?? new List<string>();
        ChangedBy = changedBy;
        ChangeReason = changeReason;
        OccurredOn = DateTime.UtcNow;
    }

    public DateTime OccurredOn { get; }
    public Guid UserId { get; }
    public string Email { get; }
    public DateTime ChangeDate { get; }
    public List<string> OldRoles { get; }
    public List<string> NewRoles { get; }
    public Guid ChangedBy { get; }
    public string ChangeReason { get; }
    public List<string> AddedRoles => NewRoles.Except(OldRoles).ToList();
    public List<string> RemovedRoles => OldRoles.Except(NewRoles).ToList();
    public bool HasChanges => AddedRoles.Any() || RemovedRoles.Any();
}
