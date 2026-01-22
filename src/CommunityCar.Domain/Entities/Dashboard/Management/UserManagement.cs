using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Dashboard.Management;

public class UserManagementAction : BaseEntity
{
    public Guid UserId { get; set; }
    public string Action { get; set; } = string.Empty; // Created, Updated, Suspended, Activated, Deleted
    public string Reason { get; set; } = string.Empty;
    public Guid PerformedBy { get; set; }
    public DateTime ActionDate { get; set; }
    public string? PreviousValues { get; set; } // JSON of previous state
    public string? NewValues { get; set; } // JSON of new state
    public string? Notes { get; set; }
    public bool IsReversible { get; set; }
    public DateTime? ExpiryDate { get; set; } // For temporary actions like suspensions

    public UserManagementAction()
    {
        ActionDate = DateTime.UtcNow;
        IsReversible = true;
    }

    public void SetTemporaryAction(DateTime expiryDate)
    {
        ExpiryDate = expiryDate;
        Audit(UpdatedBy);
    }

    public bool IsExpired()
    {
        return ExpiryDate.HasValue && ExpiryDate.Value <= DateTime.UtcNow;
    }
}