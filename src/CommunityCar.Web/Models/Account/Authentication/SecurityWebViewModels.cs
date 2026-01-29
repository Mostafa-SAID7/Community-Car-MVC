namespace CommunityCar.Web.Models.Account.Authentication;

public class SecurityOverviewWebVM
{
    public Guid UserId { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
    public int ActiveSessionsCount { get; set; }
    public DateTime? LastPasswordChange { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string? LastLoginLocation { get; set; }
    public List<SecurityLogWebVM> RecentSecurityLogs { get; set; } = new();
    public List<string> LinkedAccounts { get; set; } = new();
}

public class SecurityLogWebVM
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

public class TwoFactorSetupWebVM
{
    public string SecretKey { get; set; } = string.Empty;
    public string QrCodeUrl { get; set; } = string.Empty;
    public string ManualEntryKey { get; set; } = string.Empty;
    public List<string> BackupCodes { get; set; } = new();
    public bool IsEnabled { get; set; }
}