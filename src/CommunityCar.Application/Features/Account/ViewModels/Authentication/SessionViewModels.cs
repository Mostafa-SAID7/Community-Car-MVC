namespace CommunityCar.Application.Features.Account.ViewModels.Authentication;

public class UserSessionVM
{
    public Guid Id { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string DeviceType { get; set; } = string.Empty;
    public string Browser { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime LastActivityAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsCurrent { get; set; }
    public bool IsSuspicious { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
}

public class SessionManagementVM
{
    public Guid UserId { get; set; }
    public List<UserSessionVM> ActiveSessions { get; set; } = new();
    public List<UserSessionVM> RecentSessions { get; set; } = new();
    public int TotalActiveSessions { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string? LastLoginLocation { get; set; }
}