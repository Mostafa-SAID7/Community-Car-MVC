namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.Users.Actions;

/// <summary>
/// Bulk user action view model
/// </summary>
public class BulkUserActionVM
{
    public List<string> UserIds { get; set; } = new();
    public string Action { get; set; } = string.Empty; // Activate, Deactivate, Delete, etc.
    public string Reason { get; set; } = string.Empty;
    public DateTime ExecutedAt { get; set; }
    public string ExecutedBy { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

/// <summary>
/// Admin bulk user action view model for dashboard operations
/// </summary>
public class AdminBulkUserActionVM
{
    public List<Guid> UserIds { get; set; } = new();
    public string ActionType { get; set; } = string.Empty; // Activate, Deactivate, Delete, AssignRole, etc.
    public string ActionValue { get; set; } = string.Empty; // Role name for AssignRole, etc.
    public string Reason { get; set; } = string.Empty;
    public bool RequiresConfirmation { get; set; }
    public bool SendNotification { get; set; }
    public DateTime? ScheduledFor { get; set; }
    public string PerformedBy { get; set; } = string.Empty;
}

/// <summary>
/// Admin user moderation view model for dashboard operations
/// </summary>
public class AdminUserModerationVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty; // Warn, Suspend, Ban, etc.
    public string Reason { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
    public bool SendNotification { get; set; } = true;
    public string ModeratorNotes { get; set; } = string.Empty;
}




