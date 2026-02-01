namespace CommunityCar.Application.Features.Account.ViewModels.Management;

public class UserManagementActionVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty; // Added
    public string Reason { get; set; } = string.Empty;
    public Guid PerformedBy { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime CreatedAt => ActionDate; // Added
    public string? ManagerName { get; set; }
    public string? TargetUserName { get; set; }
    public string? ManagerProfilePicture { get; set; }
    public string? TargetUserProfilePicture { get; set; }
    public string? TimeAgo { get; set; }
    public string? ActionIcon { get; set; }
    public string? ActionColor { get; set; }
    public string? Notes { get; set; }
}