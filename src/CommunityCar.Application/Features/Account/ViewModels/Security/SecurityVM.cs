namespace CommunityCar.Application.Features.Account.ViewModels.Security;

public class SecurityVM
{
    public bool TwoFactorEnabled { get; set; }
    public int ActiveSessions { get; set; }
    public DateTime LastPasswordChange { get; set; }
    public List<string> RecentActivity { get; set; } = new();
}