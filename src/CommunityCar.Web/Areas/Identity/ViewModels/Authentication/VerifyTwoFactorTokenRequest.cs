namespace CommunityCar.Web.Areas.Identity.ViewModels.Authentication;

public class VerifyTwoFactorTokenRequest
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Provider { get; set; } = "Authenticator";
    public bool RememberMachine { get; set; }
}
