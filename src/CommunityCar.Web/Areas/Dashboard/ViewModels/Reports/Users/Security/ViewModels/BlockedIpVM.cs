namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.Users.Security.ViewModels;

/// <summary>
/// ViewModel for blocked IP addresses
/// </summary>
public class BlockedIpVM
{
    public Guid Id { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime BlockedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string BlockedBy { get; set; } = string.Empty;
    public int AttemptCount { get; set; }
    public string LastAttemptReason { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Location { get; set; } = string.Empty;
}




