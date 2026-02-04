namespace CommunityCar.Application.Features.Dashboard.Security.ViewModels;

public class BlockedIpVM
{
    public string IpAddress { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime BlockedAt { get; set; }
    public string BlockedBy { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
}