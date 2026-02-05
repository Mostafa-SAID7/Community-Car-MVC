namespace CommunityCar.Application.Features.Shared.ViewModels.Security;

/// <summary>
/// Base security settings view model - shared between Account and Dashboard
/// </summary>
public class SecuritySettingsVM
{
    public bool RequireTwoFactor { get; set; }
    public int PasswordMinLength { get; set; }
    public bool RequireSpecialCharacters { get; set; }
    public bool RequireNumbers { get; set; }
    public bool RequireUppercase { get; set; }
    public bool RequireLowercase { get; set; }
    public int MaxLoginAttempts { get; set; }
    public int LockoutDurationMinutes { get; set; }
    public int SessionTimeoutMinutes { get; set; }
    public bool EnableIpBlocking { get; set; }
    public bool EnableBruteForceProtection { get; set; }
    public bool LogSecurityEvents { get; set; }
    public bool NotifyOnSuspiciousActivity { get; set; }
    public List<string> AllowedIpRanges { get; set; } = new();
    public List<string> BlockedIpRanges { get; set; } = new();
}