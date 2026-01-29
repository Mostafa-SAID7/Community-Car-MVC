namespace CommunityCar.Web.Models.Account.Security.TwoFactor;

public class TwoFactorVM
{
    public bool IsEnabled { get; set; }
    public int RecoveryCodesLeft { get; set; }
    public bool HasRecoveryCodes { get; set; }
    public bool IsMachineRemembered { get; set; }
    public List<string> RecoveryCodes { get; set; } = new();
    public string AuthenticatorKey { get; set; } = string.Empty;
    public string AuthenticatorUri { get; set; } = string.Empty;
}