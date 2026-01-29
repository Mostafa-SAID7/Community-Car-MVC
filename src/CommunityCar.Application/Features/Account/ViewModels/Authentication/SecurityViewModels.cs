namespace CommunityCar.Application.Features.Account.ViewModels.Authentication;

public class SecurityLogVM
{
    public DateTime Timestamp { get; set; }
    public string Action { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public bool IsSuccessful { get; set; }
    public string? FailureReason { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
    public string StatusIcon { get; set; } = string.Empty;
    public string StatusColor { get; set; } = string.Empty;
}

public class TwoFactorSetupVM
{
    public string SecretKey { get; set; } = string.Empty;
    public string QrCodeUrl { get; set; } = string.Empty;
    public string ManualEntryKey { get; set; } = string.Empty;
    public List<string> BackupCodes { get; set; } = new();
    public bool IsEnabled { get; set; }
}