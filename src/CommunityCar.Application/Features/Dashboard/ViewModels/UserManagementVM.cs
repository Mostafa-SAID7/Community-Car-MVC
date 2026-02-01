namespace CommunityCar.Application.Features.Dashboard.ViewModels;

public class UserManagementVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string PerformedByName { get; set; } = string.Empty;
    public DateTime ActionDate { get; set; }
    public string ActionDateFormatted { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsBlocked { get; set; }
    public DateTime LastLogin { get; set; }
    public int LoginCount { get; set; }
    public int PostCount { get; set; }
    public int CommentCount { get; set; }
    public int ReportCount { get; set; }
    public string? Notes { get; set; }
    public bool IsReversible { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsExpired { get; set; }
    public DateTime CreatedAt { get; set; }
}