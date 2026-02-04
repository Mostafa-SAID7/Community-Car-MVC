namespace CommunityCar.Application.Features.Dashboard.UserManagement.ViewModels;

public class UserManagementActionVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime PerformedAt { get; set; }
    public string PerformedBy { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Active, Reversed, Expired
    public DateTime? ExpiresAt { get; set; }
    public DateTime? ReversedAt { get; set; }
    public string? ReversedBy { get; set; }
    public string? ReversalReason { get; set; }
    public Dictionary<string, object> ActionData { get; set; } = new();
}