namespace CommunityCar.Application.Features.Dashboard.Settings.ViewModels;

public class DashboardSettingsVM
{
    // Application Settings
    public string ApplicationName { get; set; } = string.Empty;
    public string ApplicationDescription { get; set; } = string.Empty;
    public string ApplicationVersion { get; set; } = string.Empty;
    public string ApplicationUrl { get; set; } = string.Empty;
    public string SupportEmail { get; set; } = string.Empty;
    public string AdminEmail { get; set; } = string.Empty;
    public string DefaultLanguage { get; set; } = string.Empty;
    public string DefaultTimeZone { get; set; } = string.Empty;
    
    // Registration & Authentication
    public bool EnableRegistration { get; set; }
    public bool RequireEmailConfirmation { get; set; }
    public bool EnableTwoFactorAuth { get; set; }
    public int SessionTimeoutMinutes { get; set; }
    public int MaxLoginAttempts { get; set; }
    public int LockoutDurationMinutes { get; set; }
    
    // Maintenance
    public bool EnableMaintenanceMode { get; set; }
    public string MaintenanceMessage { get; set; } = string.Empty;
    
    // Analytics & Monitoring
    public bool EnableAnalytics { get; set; }
    public string AnalyticsTrackingId { get; set; } = string.Empty;
    public bool EnableErrorReporting { get; set; }
    public string LogLevel { get; set; } = string.Empty;
    
    // Performance
    public bool EnableCaching { get; set; }
    public int CacheExpirationMinutes { get; set; }
    public bool EnableCompression { get; set; }
    
    // File Upload
    public int MaxFileUploadSizeMB { get; set; }
    public string AllowedFileTypes { get; set; } = string.Empty;
    
    // SMTP Settings
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public string SmtpUsername { get; set; } = string.Empty;
    public bool SmtpEnableSsl { get; set; }
    
    // Notifications
    public bool EnableNotifications { get; set; }
    public bool EnableEmailNotifications { get; set; }
    public bool EnablePushNotifications { get; set; }
    public bool EnableSmsNotifications { get; set; }
    
    // Appearance
    public string ThemeName { get; set; } = string.Empty;
    public string PrimaryColor { get; set; } = string.Empty;
    public string SecondaryColor { get; set; } = string.Empty;
    public bool EnableDarkMode { get; set; }
    
    // UI Features
    public bool ShowBreadcrumbs { get; set; }
    public bool ShowSidebar { get; set; }
    public bool EnableSearch { get; set; }
    public int ItemsPerPage { get; set; }
    
    // Content Features
    public bool EnableComments { get; set; }
    public bool EnableRatings { get; set; }
    public bool EnableSharing { get; set; }
    public bool EnableBookmarks { get; set; }
    public bool EnableTags { get; set; }
    public bool EnableCategories { get; set; }
    
    // Moderation
    public string ModerationMode { get; set; } = string.Empty;
    public bool RequireApproval { get; set; }
    public bool EnableSpamFilter { get; set; }
    public bool EnableProfanityFilter { get; set; }
    public int MaxCommentLength { get; set; }
    public int MaxPostLength { get; set; }
    
    // Location & Maps
    public bool EnableGeoLocation { get; set; }
    public int DefaultMapZoom { get; set; }
    public string MapProvider { get; set; } = string.Empty;
    
    // Widgets
    public bool EnableWeatherWidget { get; set; }
    public bool EnableNewsWidget { get; set; }
    public bool EnableCalendarWidget { get; set; }
    public bool EnableChatWidget { get; set; }
    
    // Backup
    public string BackupFrequency { get; set; } = string.Empty;
    public int BackupRetentionDays { get; set; }
    public bool EnableAutoBackup { get; set; }
    
    // Infrastructure
    public string DatabaseProvider { get; set; } = string.Empty;
    public string CacheProvider { get; set; } = string.Empty;
    public string FileStorageProvider { get; set; } = string.Empty;
    
    // CDN & SSL
    public bool EnableCdn { get; set; }
    public string CdnUrl { get; set; } = string.Empty;
    public bool EnableSsl { get; set; }
    public bool EnableHsts { get; set; }
    
    // CORS & Security
    public bool EnableCors { get; set; }
    public string AllowedOrigins { get; set; } = string.Empty;
    
    // Rate Limiting
    public bool EnableRateLimiting { get; set; }
    public int RateLimitRequests { get; set; }
    public int RateLimitWindow { get; set; }
    
    // IP & Geo Blocking
    public bool EnableIpBlocking { get; set; }
    public bool EnableGeoBlocking { get; set; }
    public string BlockedCountries { get; set; } = string.Empty;
    
    // Audit & Monitoring
    public bool EnableAuditLogging { get; set; }
    public int AuditLogRetentionDays { get; set; }
    public bool EnablePerformanceMonitoring { get; set; }
    public bool EnableHealthChecks { get; set; }
    public int HealthCheckInterval { get; set; }
    
    // Alerts
    public bool EnableAlerts { get; set; }
    public string AlertEmail { get; set; } = string.Empty;
    public int CriticalAlertThreshold { get; set; }
    public int WarningAlertThreshold { get; set; }

    public GeneralSettingsVM General { get; set; } = new();
    public SecuritySettingsVM Security { get; set; } = new();
    public NotificationSettingsVM Notifications { get; set; } = new();
    public PerformanceSettingsVM Performance { get; set; } = new();
    public MaintenanceSettingsVM Maintenance { get; set; } = new();
    public BackupSettingsVM Backup { get; set; } = new();
    public LoggingSettingsVM Logging { get; set; } = new();
}

public class GeneralSettingsVM
{
    public string SiteName { get; set; } = string.Empty;
    public string SiteDescription { get; set; } = string.Empty;
    public string SiteUrl { get; set; } = string.Empty;
    public string AdminEmail { get; set; } = string.Empty;
    public string TimeZone { get; set; } = string.Empty;
    public string DefaultLanguage { get; set; } = string.Empty;
    public bool MaintenanceMode { get; set; }
    public string MaintenanceMessage { get; set; } = string.Empty;
}

public class SecuritySettingsVM
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

public class NotificationSettingsVM
{
    public bool EnableEmailNotifications { get; set; }
    public bool EnablePushNotifications { get; set; }
    public bool EnableSmsNotifications { get; set; }
    public bool NotifyOnNewUser { get; set; }
    public bool NotifyOnSecurityAlert { get; set; }
    public bool NotifyOnSystemError { get; set; }
    public bool NotifyOnMaintenanceMode { get; set; }
    public List<string> AdminEmails { get; set; } = new();
}

public class PerformanceSettingsVM
{
    public bool EnableCaching { get; set; }
    public int CacheExpirationMinutes { get; set; }
    public bool EnableCompression { get; set; }
    public bool EnableMinification { get; set; }
    public bool EnableCdn { get; set; }
    public string CdnUrl { get; set; } = string.Empty;
    public int MaxRequestSize { get; set; }
    public int RequestTimeout { get; set; }
}

public class MaintenanceSettingsVM
{
    public bool AutoBackup { get; set; }
    public int BackupRetentionDays { get; set; }
    public bool AutoCleanupLogs { get; set; }
    public int LogRetentionDays { get; set; }
    public bool AutoOptimizeDatabase { get; set; }
    public string MaintenanceWindow { get; set; } = string.Empty;
}

public class BackupSettingsVM
{
    public bool EnableAutoBackup { get; set; }
    public string BackupFrequency { get; set; } = string.Empty;
    public int RetentionDays { get; set; }
    public string BackupLocation { get; set; } = string.Empty;
    public bool IncludeDatabase { get; set; }
    public bool IncludeFiles { get; set; }
    public bool CompressBackups { get; set; }
    public bool EncryptBackups { get; set; }
}

public class LoggingSettingsVM
{
    public string LogLevel { get; set; } = string.Empty;
    public bool EnableFileLogging { get; set; }
    public bool EnableDatabaseLogging { get; set; }
    public bool EnableRemoteLogging { get; set; }
    public string RemoteLoggingUrl { get; set; } = string.Empty;
    public int MaxLogFileSize { get; set; }
    public int LogRetentionDays { get; set; }
}

public class SettingsCategoryVM
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public int Order { get; set; }
    public List<SettingItemVM> Settings { get; set; } = new();
}

public class SettingItemVM
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public object Value { get; set; } = new();
    public object DefaultValue { get; set; } = new();
    public bool IsRequired { get; set; }
    public string ValidationRules { get; set; } = string.Empty;
    public List<SettingOptionVM> Options { get; set; } = new();
}

public class SettingOptionVM
{
    public string Value { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}