namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.Users.Security.ViewModels;

/// <summary>
/// ViewModel for security settings
/// </summary>
public class SecuritySettingsVM
{
    public bool RequireTwoFactor { get; set; }
    public bool EnablePasswordComplexity { get; set; }
    public int PasswordMinLength { get; set; }
    public int PasswordExpirationDays { get; set; }
    public int MaxFailedLoginAttempts { get; set; }
    public int AccountLockoutDuration { get; set; }
    public bool EnableIpWhitelist { get; set; }
    public bool EnableSessionTimeout { get; set; }
    public int SessionTimeoutMinutes { get; set; }
    public bool LogSecurityEvents { get; set; }
    public bool EnableBruteForceProtection { get; set; }
    public bool RequireEmailVerification { get; set; }
    public bool EnableCaptcha { get; set; }
    public bool EnableRateLimiting { get; set; }
    public int RateLimitRequests { get; set; }
    public int RateLimitWindow { get; set; }
    public List<string> AllowedIpAddresses { get; set; } = new();
    public List<string> BlockedIpAddresses { get; set; } = new();
    public List<SecurityRuleSettingVM> SecurityRules { get; set; } = new();
}

/// <summary>
/// ViewModel for security rule setting
/// </summary>
public class SecurityRuleSettingVM
{
    public string RuleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public string RuleType { get; set; } = string.Empty;
    public Dictionary<string, object> RuleParameters { get; set; } = new();
}




