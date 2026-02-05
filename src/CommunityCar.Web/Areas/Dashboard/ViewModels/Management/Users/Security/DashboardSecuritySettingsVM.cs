namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.Users.Security;

/// <summary>
/// ViewModel for dashboard security settings
/// </summary>
public class DashboardSecuritySettingsVM
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
    public List<string> AllowedIpAddresses { get; set; } = new();
    public List<SecurityRuleVM> SecurityRules { get; set; } = new();
}

/// <summary>
/// ViewModel for security rules
/// </summary>
public class SecurityRuleVM
{
    public string RuleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public string RuleType { get; set; } = string.Empty;
    public Dictionary<string, object> RuleParameters { get; set; } = new();
}




