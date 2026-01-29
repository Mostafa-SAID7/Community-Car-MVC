namespace CommunityCar.Infrastructure.Configuration.Account.Core;

/// <summary>
/// Core account configuration settings
/// </summary>
public class AccountSettings
{
    public const string SectionName = "Account";

    public bool RequireEmailConfirmation { get; set; } = true;
    public bool RequirePhoneConfirmation { get; set; } = false;
    public bool AllowUserRegistration { get; set; } = true;
    public bool AllowGuestAccess { get; set; } = false;
    public TimeSpan AccountActivationTokenExpiry { get; set; } = TimeSpan.FromDays(7);
    public TimeSpan PasswordResetTokenExpiry { get; set; } = TimeSpan.FromHours(24);
    public int MaxLoginAttempts { get; set; } = 5;
    public TimeSpan LoginLockoutDuration { get; set; } = TimeSpan.FromMinutes(15);
    
    public ProfileSettings Profile { get; set; } = new();
    public AccountSecuritySettings Security { get; set; } = new();
    public AccountNotificationSettings Notifications { get; set; } = new();
    public AccountPrivacySettings Privacy { get; set; } = new();
}

/// <summary>
/// Profile-related account settings
/// </summary>
public class ProfileSettings
{
    public bool AllowProfilePictureUpload { get; set; } = true;
    public bool AllowCoverImageUpload { get; set; } = true;
    public long MaxProfilePictureSize { get; set; } = 5 * 1024 * 1024; // 5MB
    public long MaxCoverImageSize { get; set; } = 10 * 1024 * 1024; // 10MB
    public string[] AllowedImageFormats { get; set; } = { "jpg", "jpeg", "png", "gif", "webp" };
    public bool RequireProfileCompletion { get; set; } = false;
    public int MinBioLength { get; set; } = 0;
    public int MaxBioLength { get; set; } = 500;
    public bool AllowUsernameChange { get; set; } = true;
    public TimeSpan UsernameChangeInterval { get; set; } = TimeSpan.FromDays(30);
}

/// <summary>
/// Security-related account settings
/// </summary>
public class AccountSecuritySettings
{
    public bool EnableTwoFactorAuthentication { get; set; } = true;
    public bool RequireTwoFactorForAdmins { get; set; } = true;
    public TimeSpan TwoFactorTokenExpiry { get; set; } = TimeSpan.FromMinutes(5);
    public int MaxTwoFactorAttempts { get; set; } = 3;
    public TimeSpan TwoFactorLockoutDuration { get; set; } = TimeSpan.FromMinutes(15);
    public bool EnableSessionManagement { get; set; } = true;
    public TimeSpan SessionTimeout { get; set; } = TimeSpan.FromHours(24);
    public int MaxConcurrentSessions { get; set; } = 5;
    public bool LogSecurityEvents { get; set; } = true;
    public bool EnableSuspiciousActivityDetection { get; set; } = true;
}

/// <summary>
/// Notification-related account settings
/// </summary>
public class AccountNotificationSettings
{
    public bool EnableEmailNotifications { get; set; } = true;
    public bool EnablePushNotifications { get; set; } = true;
    public bool EnableSmsNotifications { get; set; } = false;
    public bool AllowMarketingEmails { get; set; } = false;
    public bool NotifyOnLogin { get; set; } = true;
    public bool NotifyOnPasswordChange { get; set; } = true;
    public bool NotifyOnProfileUpdate { get; set; } = false;
    public TimeSpan NotificationBatchInterval { get; set; } = TimeSpan.FromMinutes(15);
}

/// <summary>
/// Privacy-related account settings
/// </summary>
public class AccountPrivacySettings
{
    public bool DefaultProfileVisibility { get; set; } = true; // true = public, false = private
    public bool AllowSearchEngineIndexing { get; set; } = true;
    public bool ShowOnlineStatus { get; set; } = true;
    public bool ShowLastSeen { get; set; } = true;
    public bool AllowDirectMessages { get; set; } = true;
    public bool ShowEmail { get; set; } = false;
    public bool ShowPhoneNumber { get; set; } = false;
    public TimeSpan DataRetentionPeriod { get; set; } = TimeSpan.FromDays(365 * 7); // 7 years
    public bool EnableDataExport { get; set; } = true;
    public bool EnableAccountDeletion { get; set; } = true;
}