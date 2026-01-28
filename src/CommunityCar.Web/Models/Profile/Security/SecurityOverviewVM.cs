using CommunityCar.Web.Models.Account.External;

namespace CommunityCar.Web.Models.Profile.Security;

public class SecurityOverviewVM
{
    public bool IsTwoFactorEnabled { get; set; }
    public DateTime? LastPasswordChange { get; set; }
    public int ActiveSessions { get; set; }
    public bool HasLinkedAccounts { get; set; }
    public List<LinkedAccountVM> LinkedAccounts { get; set; } = new();
    public List<SecurityLogItemVM> RecentSecurityLogs { get; set; } = new();
    public bool HasRecoveryCodes { get; set; }
    public int RecoveryCodesLeft { get; set; }
}