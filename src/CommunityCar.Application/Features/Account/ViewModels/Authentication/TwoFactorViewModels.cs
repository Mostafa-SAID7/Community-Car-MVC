namespace CommunityCar.Application.Features.Account.ViewModels.Authentication;

public class TwoFactorSetupVM
{
    public string SecretKey { get; set; } = string.Empty;
    public string QrCodeUrl { get; set; } = string.Empty;
    public string ManualEntryKey { get; set; } = string.Empty;
    public List<string> BackupCodes { get; set; } = new();
    public bool IsEnabled { get; set; }
}