using CommunityCar.Application.Features.Dashboard.Management.developer.System;
using CommunityCar.Application.Features.Dashboard.Management.developer.System.ViewModels;
using CommunityCar.Application.Features.Dashboard.Management.developer.Performance.ViewModels;
using CommunityCar.Application.Features.Dashboard.Management.developer.Maintenance.ViewModels;

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
    
    // Missing properties that services expect
    public string SiteName { get; set; } = string.Empty;
    public string SiteDescription { get; set; } = string.Empty;
    public bool MaintenanceMode { get; set; }
    public bool AllowRegistration { get; set; }
    public bool RequireEmailConfirmation { get; set; }
    
    // Registration & Authentication
    public bool EnableRegistration { get; set; }
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

    // Nested Settings
    public GeneralSettingsVM General { get; set; } = new();
    public DashboardSecuritySettingsVM Security { get; set; } = new();
    public DashboardNotificationSettingsVM Notifications { get; set; } = new();
    public PerformanceSettingsVM Performance { get; set; } = new();
    public MaintenanceSettingsVM Maintenance { get; set; } = new();
    public BackupSettingsVM Backup { get; set; } = new();
    public LoggingSettingsVM Logging { get; set; } = new();
}