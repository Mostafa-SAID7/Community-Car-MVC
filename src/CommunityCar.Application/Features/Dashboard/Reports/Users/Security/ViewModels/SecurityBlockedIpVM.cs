namespace CommunityCar.Application.Features.Dashboard.Reports.Users.Security.ViewModels;

/// <summary>
/// Security blocked IP view model
/// </summary>
public class SecurityBlockedIpVM
{
    public Guid Id { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime BlockedAt { get; set; }
    public string BlockedBy { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
    public int AttemptCount { get; set; }
    public string LastAttemptReason { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}