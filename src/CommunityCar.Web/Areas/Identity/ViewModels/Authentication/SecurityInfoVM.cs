namespace CommunityCar.Web.Areas.Identity.ViewModels.Authentication;

public class SecurityInfoVM
{
    public bool IsTwoFactorEnabled { get; set; }
    public DateTime? LastPasswordChange { get; set; }
    public int ActiveSessions { get; set; }
    public bool HasOAuthLinked { get; set; }
}
