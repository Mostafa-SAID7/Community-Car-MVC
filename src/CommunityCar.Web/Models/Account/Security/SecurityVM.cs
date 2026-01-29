using CommunityCar.Web.Models.Account.Authentication.Login.External;

namespace CommunityCar.Web.Models.Account.Security;

/// <summary>
/// View model for security overview page
/// </summary>
public class SecurityVM
{
    public bool IsTwoFactorEnabled { get; set; }
    public DateTime? LastPasswordChange { get; set; }
    public int ActiveSessions { get; set; }
    public DateTime? LastLogin { get; set; }
    public string? LastLoginIp { get; set; }
    public bool IsAccountLocked { get; set; }
    public int FailedLoginAttempts { get; set; }
    public int RecoveryCodesCount { get; set; }
    public List<ExternalLoginDisplayVM> ExternalLogins { get; set; } = new();
    public List<SecurityLogItemVM> RecentActivity { get; set; } = new();
}