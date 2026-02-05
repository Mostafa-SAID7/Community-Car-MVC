namespace CommunityCar.Application.Features.Shared.ViewModels.Security;

/// <summary>
/// Base security overview view model - shared between Account and Dashboard
/// </summary>
public class SecurityOverviewVM
{
    public int TotalThreats { get; set; }
    public int BlockedAttacks { get; set; }
    public int FailedLogins { get; set; }
    public int SuspiciousActivities { get; set; }
    public decimal SecurityScore { get; set; }
    public DateTime LastSecurityScan { get; set; }
    public int TwoFactorEnabled { get; set; }
    public int PasswordStrengthAverage { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
    public int ActiveSessions { get; set; }
    public DateTime LastPasswordChange { get; set; }
    public DateTime LastLogin { get; set; }
    public string LastLoginLocation { get; set; } = string.Empty;
    public List<string> RecentSecurityEvents { get; set; } = new();
}