using CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.Users.Security.ViewModels;
using CommunityCar.Application.Features.Shared.ViewModels.Security;

namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.Users.Security;

/// <summary>
/// ViewModel for user security management
/// </summary>
public class UserSecurityManagementVM
{
    // Missing properties that services expect
    public Guid UserId { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
    public bool IsLocked { get; set; }
    public int FailedLoginAttempts { get; set; }
    
    public List<UserSecurityProfileVM> SecurityProfiles { get; set; } = new();
    public List<SecurityThreatVM> ActiveThreats { get; set; } = new();
    public List<BlockedIpVM> BlockedIps { get; set; } = new();
    public List<SuspiciousActivityVM> SuspiciousActivities { get; set; } = new();
    public SecurityManagementStatsVM Stats { get; set; } = new();
    public List<SecurityActionVM> AvailableActions { get; set; } = new();
}

/// <summary>
/// ViewModel for user security profile
/// </summary>
public class UserSecurityProfileVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SecurityLevel { get; set; } = string.Empty; // Low, Medium, High, Critical
    public decimal SecurityScore { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public bool HasSecurityIssues { get; set; }
    public int FailedLoginAttempts { get; set; }
    public int SuspiciousActivities { get; set; }
    public DateTime LastSecurityCheck { get; set; }
    public DateTime LastPasswordChange { get; set; }
    public List<string> SecurityFlags { get; set; } = new();
    public List<SecurityEventVM> RecentEvents { get; set; } = new();
}

/// <summary>
/// ViewModel for blocked IP addresses
/// </summary>
public class BlockedIpVM
{
    public string IpAddress { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string ThreatLevel { get; set; } = string.Empty;
    public DateTime BlockedAt { get; set; }
    public string BlockedBy { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsPermanent { get; set; }
    public int AttemptCount { get; set; }
    public string Location { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public List<string> AssociatedUsers { get; set; } = new();
    public List<SecurityEventVM> RelatedEvents { get; set; } = new();
}

/// <summary>
/// ViewModel for security events
/// </summary>
public class SecurityEventVM
{
    public Guid Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public bool IsSuccessful { get; set; }
    public string? FailureReason { get; set; }
    public string Location { get; set; } = string.Empty;
    public Dictionary<string, object> EventData { get; set; } = new();
}

/// <summary>
/// ViewModel for security management statistics
/// </summary>
public class SecurityManagementStatsVM
{
    public int ActiveThreats { get; set; }
    public int BlockedIps { get; set; }
    public int SuspiciousActivities { get; set; }
    public int SecurityEvents { get; set; }
    public int UsersWithIssues { get; set; }
    public int TwoFactorEnabledUsers { get; set; }
    public decimal OverallSecurityScore { get; set; }
    public int FailedLoginAttempts { get; set; }
    public int SuccessfulLogins { get; set; }
    public int PasswordResets { get; set; }
    public int AccountLockouts { get; set; }
}

/// <summary>
/// ViewModel for security actions
/// </summary>
public class SecurityActionVM
{
    public string ActionId { get; set; } = string.Empty;
    public string ActionName { get; set; } = string.Empty;
    public string ActionDescription { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty; // User, IP, System
    public string Severity { get; set; } = string.Empty;
    public bool RequiresConfirmation { get; set; }
    public bool RequiresReason { get; set; }
    public List<string> RequiredPermissions { get; set; } = new();
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}




