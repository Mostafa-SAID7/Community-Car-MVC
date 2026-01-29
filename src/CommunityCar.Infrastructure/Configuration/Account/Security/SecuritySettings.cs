namespace CommunityCar.Infrastructure.Configuration.Account.Security;

/// <summary>
/// Security configuration settings for accounts
/// </summary>
public class SecuritySettings
{
    public const string SectionName = "Security";

    public PasswordPolicySettings PasswordPolicy { get; set; } = new();
    public LockoutPolicySettings LockoutPolicy { get; set; } = new();
    public AuditSettings Audit { get; set; } = new();
    public EncryptionSettings Encryption { get; set; } = new();
    public RateLimitingSettings RateLimiting { get; set; } = new();
}

/// <summary>
/// Password policy configuration settings
/// </summary>
public class PasswordPolicySettings
{
    public int MinLength { get; set; } = 8;
    public int MaxLength { get; set; } = 128;
    public bool RequireUppercase { get; set; } = true;
    public bool RequireLowercase { get; set; } = true;
    public bool RequireDigit { get; set; } = true;
    public bool RequireSpecialChar { get; set; } = true;
    public int PreventPasswordReuse { get; set; } = 5;
    public bool PreventSimilarPasswords { get; set; } = true;
    public bool PreventPersonalInfo { get; set; } = true;
    public TimeSpan MinPasswordAge { get; set; } = TimeSpan.FromDays(1);
    public TimeSpan MaxPasswordAge { get; set; } = TimeSpan.FromDays(90);
    public TimeSpan ExpirationWarningPeriod { get; set; } = TimeSpan.FromDays(14);
    public bool CheckCommonPasswords { get; set; } = true;
    public bool CheckBreachedPasswords { get; set; } = true;

    public static PasswordPolicySettings Default => new();

    public static PasswordPolicySettings Strict => new()
    {
        MinLength = 12,
        RequireUppercase = true,
        RequireLowercase = true,
        RequireDigit = true,
        RequireSpecialChar = true,
        PreventPasswordReuse = 10,
        PreventSimilarPasswords = true,
        PreventPersonalInfo = true,
        MinPasswordAge = TimeSpan.FromDays(1),
        MaxPasswordAge = TimeSpan.FromDays(60),
        ExpirationWarningPeriod = TimeSpan.FromDays(7),
        CheckCommonPasswords = true,
        CheckBreachedPasswords = true
    };

    public static PasswordPolicySettings Relaxed => new()
    {
        MinLength = 6,
        RequireUppercase = false,
        RequireLowercase = true,
        RequireDigit = false,
        RequireSpecialChar = false,
        PreventPasswordReuse = 3,
        PreventSimilarPasswords = false,
        PreventPersonalInfo = false,
        MinPasswordAge = TimeSpan.Zero,
        MaxPasswordAge = TimeSpan.Zero,
        ExpirationWarningPeriod = TimeSpan.Zero,
        CheckCommonPasswords = false,
        CheckBreachedPasswords = false
    };
}

/// <summary>
/// Account lockout policy settings
/// </summary>
public class LockoutPolicySettings
{
    public int MaxFailedAttempts { get; set; } = 5;
    public TimeSpan LockoutDuration { get; set; } = TimeSpan.FromMinutes(15);
    public TimeSpan SecurityLockoutDuration { get; set; } = TimeSpan.FromHours(1);
    public bool LockoutOnSuspiciousActivity { get; set; } = true;
    public int MaxLockoutMultiplier { get; set; } = 8;
    public string[] ExemptRoles { get; set; } = { "SuperAdmin" };
    public bool EnableProgressiveLockout { get; set; } = true;
    public TimeSpan LockoutResetPeriod { get; set; } = TimeSpan.FromHours(24);

    public static LockoutPolicySettings Default => new();

    public static LockoutPolicySettings Strict => new()
    {
        MaxFailedAttempts = 3,
        LockoutDuration = TimeSpan.FromMinutes(30),
        SecurityLockoutDuration = TimeSpan.FromHours(2),
        LockoutOnSuspiciousActivity = true,
        MaxLockoutMultiplier = 16,
        ExemptRoles = Array.Empty<string>(),
        EnableProgressiveLockout = true,
        LockoutResetPeriod = TimeSpan.FromHours(12)
    };

    public static LockoutPolicySettings Relaxed => new()
    {
        MaxFailedAttempts = 10,
        LockoutDuration = TimeSpan.FromMinutes(5),
        SecurityLockoutDuration = TimeSpan.FromMinutes(30),
        LockoutOnSuspiciousActivity = false,
        MaxLockoutMultiplier = 2,
        ExemptRoles = new[] { "SuperAdmin", "Admin", "Moderator" },
        EnableProgressiveLockout = false,
        LockoutResetPeriod = TimeSpan.FromHours(48)
    };
}

/// <summary>
/// Security audit settings
/// </summary>
public class AuditSettings
{
    public bool EnableAuditLogging { get; set; } = true;
    public bool LogSuccessfulLogins { get; set; } = true;
    public bool LogFailedLogins { get; set; } = true;
    public bool LogPasswordChanges { get; set; } = true;
    public bool LogProfileUpdates { get; set; } = false;
    public bool LogPermissionChanges { get; set; } = true;
    public bool LogSuspiciousActivity { get; set; } = true;
    public TimeSpan AuditLogRetentionPeriod { get; set; } = TimeSpan.FromDays(365);
    public bool EnableRealTimeAlerts { get; set; } = true;
    public string[] AlertRecipients { get; set; } = Array.Empty<string>();
}

/// <summary>
/// Encryption settings for sensitive data
/// </summary>
public class EncryptionSettings
{
    public string EncryptionKey { get; set; } = string.Empty;
    public string HashAlgorithm { get; set; } = "SHA256";
    public int SaltLength { get; set; } = 32;
    public int HashIterations { get; set; } = 10000;
    public bool EncryptPersonalData { get; set; } = true;
    public bool EncryptCommunications { get; set; } = true;
    public TimeSpan KeyRotationInterval { get; set; } = TimeSpan.FromDays(90);
}

/// <summary>
/// Rate limiting settings for security
/// </summary>
public class RateLimitingSettings
{
    public bool EnableRateLimiting { get; set; } = true;
    public int LoginAttemptsPerMinute { get; set; } = 5;
    public int PasswordResetAttemptsPerHour { get; set; } = 3;
    public int RegistrationAttemptsPerHour { get; set; } = 10;
    public int ProfileUpdateAttemptsPerHour { get; set; } = 20;
    public TimeSpan RateLimitWindow { get; set; } = TimeSpan.FromMinutes(1);
    public string[] ExemptIpAddresses { get; set; } = Array.Empty<string>();
    public bool EnableIpWhitelist { get; set; } = false;
    public string[] IpWhitelist { get; set; } = Array.Empty<string>();
    public bool EnableIpBlacklist { get; set; } = true;
    public string[] IpBlacklist { get; set; } = Array.Empty<string>();
}