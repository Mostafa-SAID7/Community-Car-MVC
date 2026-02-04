namespace CommunityCar.Application.Features.Account.ViewModels.Security;

/// <summary>
/// ViewModel for account security overview
/// </summary>
public class AccountSecurityOverviewVM
{
    public Guid UserId { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public bool IsPhoneConfirmed { get; set; }
    public DateTime? LastPasswordChange { get; set; }
    public DateTime? LastLogin { get; set; }
    public int ActiveSessions { get; set; }
    public int RecentLoginAttempts { get; set; }
    public int FailedLoginAttempts { get; set; }
    public bool HasRecentSuspiciousActivity { get; set; }
    public List<LoginHistoryVM> RecentLogins { get; set; } = new();
    public List<SecurityEventVM> RecentSecurityEvents { get; set; } = new();
    public SecurityScoreVM SecurityScore { get; set; } = new();
}