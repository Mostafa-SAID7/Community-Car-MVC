namespace CommunityCar.Application.Features.Dashboard.Management.Users.Actions;

/// <summary>
/// Admin user management action view model for dashboard operations
/// </summary>
public class AdminUserManagementActionVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public Guid PerformedBy { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime CreatedAt => ActionDate;
    public string? ManagerName { get; set; }
    public string? TargetUserName { get; set; }
    public string? ManagerProfilePicture { get; set; }
    public string? TargetUserProfilePicture { get; set; }
    public string? TimeAgo { get; set; }
    public string? ActionIcon { get; set; }
    public string? ActionColor { get; set; }
    public string? Notes { get; set; }
    public string? AdminComments { get; set; }
    public bool RequiresApproval { get; set; }
    public bool IsReversible { get; set; }
}