namespace CommunityCar.Application.Features.Account.ViewModels.Security;

public class SecurityOverviewVM
{
    public bool TwoFactorEnabled { get; set; }
    public int ActiveSessionsCount { get; set; }
    public DateTime? LastPasswordChange { get; set; }
    public bool HasExternalLogins { get; set; }
    public List<SecurityLogItemVM> RecentActivity { get; set; } = new();
}