namespace CommunityCar.Web.Models.Profile.Security.TwoFactor;

public class EnableTwoFactorVM
{
    public string QrCodeUri { get; set; } = string.Empty;
    public string ManualEntryKey { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string VerificationCode { get; set; } = string.Empty;
    public List<string> RecoveryCodes { get; set; } = new();
}