using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Account.Management;

public class UserManagementAction : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid TargetUserId => UserId; // Alias for repository
    public string Action { get; private set; } = string.Empty; // Created, Updated, Suspended, Activated, Deleted
    public string ActionType => Action; // Alias for repository
    public string Description { get; private set; } = string.Empty;
    public string Reason { get; private set; } = string.Empty;
    public Guid PerformedBy { get; private set; }
    public Guid ManagerId => PerformedBy; // Alias for repository
    public string? Metadata { get; private set; } // JSON for additional data
    public DateTime ActionDate { get; private set; }
    public string? PreviousValues { get; private set; } // JSON of previous state
    public string? NewValues { get; private set; } // JSON of new state
    public string? Notes { get; private set; }
    public bool IsReversible { get; private set; }
    public DateTime? ExpiryDate { get; private set; } // For temporary actions like suspensions

    public UserManagementAction(
        Guid userId,
        string action,
        string description,
        string reason,
        Guid performedBy,
        string? previousValues = null,
        string? newValues = null,
        string? notes = null,
        bool isReversible = true)
    {
        UserId = userId;
        Action = action;
        Description = description;
        Reason = reason;
        PerformedBy = performedBy;
        PreviousValues = previousValues;
        NewValues = newValues;
        Notes = notes;
        IsReversible = isReversible;
        ActionDate = DateTime.UtcNow;
    }

    public static UserManagementAction Create(Guid userId, Guid performedBy, string action, string description)
    {
        return new UserManagementAction(userId, action, description, "General", performedBy);
    }

    private UserManagementAction() 
    {
        ActionDate = DateTime.UtcNow;
        IsReversible = true;
    }

    public void SetTemporaryAction(DateTime expiryDate)
    {
        ExpiryDate = expiryDate;
        Audit(UpdatedBy);
    }

    public void UpdateNotes(string notes)
    {
        Notes = notes;
        Audit(UpdatedBy);
    }

    public bool IsExpired()
    {
        return ExpiryDate.HasValue && ExpiryDate.Value <= DateTime.UtcNow;
    }
}