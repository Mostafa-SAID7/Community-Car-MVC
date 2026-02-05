namespace CommunityCar.Application.Features.Dashboard.Settings.ViewModels;

/// <summary>
/// Dashboard security settings view model
/// </summary>
public class DashboardSecuritySettingsVM
{
    public bool RequireTwoFactor { get; set; }
    public int PasswordMinLength { get; set; }
    public bool PasswordRequireUppercase { get; set; }
    public bool PasswordRequireLowercase { get; set; }
    public bool PasswordRequireNumbers { get; set; }
    public bool PasswordRequireSymbols { get; set; }
    public int MaxFailedLoginAttempts { get; set; }
    public int AccountLockoutDuration { get; set; }
    public int SessionTimeout { get; set; }
    public bool RequireEmailConfirmation { get; set; }
    public bool EnableSecurityHeaders { get; set; }
    public bool EnableRateLimiting { get; set; }
    public int RateLimitRequests { get; set; }
    public int RateLimitWindow { get; set; }
    public bool EnableIpBlocking { get; set; }
    public bool AutoBlockSuspiciousIps { get; set; }
}