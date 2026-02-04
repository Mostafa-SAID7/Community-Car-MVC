namespace CommunityCar.Application.Features.Dashboard.Security.ViewModels;

public class SecuritySettingsVM
{
    public bool RequireTwoFactor { get; set; }
    public bool EnableTwoFactorAuth { get; set; }
    public bool RequireStrongPasswords { get; set; }
    public int PasswordMinLength { get; set; }
    public bool PasswordRequireUppercase { get; set; }
    public bool PasswordRequireLowercase { get; set; }
    public bool PasswordRequireNumbers { get; set; }
    public bool PasswordRequireSymbols { get; set; }
    public int MaxFailedLoginAttempts { get; set; }
    public int MaxLoginAttempts { get; set; }
    public int AccountLockoutDuration { get; set; }
    public int LockoutDuration { get; set; }
    public int SessionTimeout { get; set; }
    public bool RequireEmailConfirmation { get; set; }
    public bool EnableIpBlocking { get; set; }
    public bool AutoBlockSuspiciousIps { get; set; }
    public bool EnableRateLimiting { get; set; }
    public int RateLimitRequests { get; set; }
    public int RateLimitWindow { get; set; }
    public bool EnableSecurityHeaders { get; set; }
    public bool EnableCsrfProtection { get; set; }
    public bool EnableXssProtection { get; set; }
    public bool EnableSqlInjectionProtection { get; set; }
    public List<string> TrustedIpAddresses { get; set; } = new();
    public List<string> BlockedIpAddresses { get; set; } = new();
    public Dictionary<string, object> AdvancedSettings { get; set; } = new();
}