namespace CommunityCar.Infrastructure.Configuration.Account.Management;

/// <summary>
/// Account management configuration settings
/// </summary>
public class ManagementSettings
{
    public const string SectionName = "Management";

    public AdminSettings Admin { get; set; } = new();
    public ModerationSettings Moderation { get; set; } = new();
    public AnalyticsSettings Analytics { get; set; } = new();
    public MaintenanceSettings Maintenance { get; set; } = new();
    public ComplianceSettings Compliance { get; set; } = new();
}

/// <summary>
/// Administrative settings
/// </summary>
public class AdminSettings
{
    public bool EnableAdminPanel { get; set; } = true;
    public bool RequireTwoFactorForAdmins { get; set; } = true;
    public bool EnableAdminAuditLog { get; set; } = true;
    public bool EnableAdminNotifications { get; set; } = true;
    public string[] AdminEmailAddresses { get; set; } = Array.Empty<string>();
    public bool EnableBulkOperations { get; set; } = true;
    public int MaxBulkOperationSize { get; set; } = 1000;
    public bool EnableUserImpersonation { get; set; } = false;
    public bool LogUserImpersonation { get; set; } = true;
    public TimeSpan AdminSessionTimeout { get; set; } = TimeSpan.FromHours(8);
    public bool EnableAdminApiAccess { get; set; } = true;
    public bool RequireAdminApprovalForActions { get; set; } = false;
    public string[] ActionsRequiringApproval { get; set; } = { "DeleteUser", "BanUser", "ModifyPermissions" };
}

/// <summary>
/// Content moderation settings
/// </summary>
public class ModerationSettings
{
    public bool EnableContentModeration { get; set; } = true;
    public bool EnableAutoModeration { get; set; } = true;
    public bool EnableManualModeration { get; set; } = true;
    public bool EnableCommunityModeration { get; set; } = false;
    public bool RequireApprovalForNewUsers { get; set; } = false;
    public bool RequireApprovalForPosts { get; set; } = false;
    public bool RequireApprovalForComments { get; set; } = false;
    public bool EnableProfanityFilter { get; set; } = true;
    public bool EnableSpamDetection { get; set; } = true;
    public bool EnableToxicityDetection { get; set; } = true;
    public double ToxicityThreshold { get; set; } = 0.7; // 0.0 - 1.0
    public bool EnableImageModeration { get; set; } = true;
    public bool EnableVideoModeration { get; set; } = false;
    public int ModerationQueueSize { get; set; } = 1000;
    public TimeSpan ModerationResponseTime { get; set; } = TimeSpan.FromHours(24);
    public bool EnableModerationNotifications { get; set; } = true;
    public string[] ModeratorEmailAddresses { get; set; } = Array.Empty<string>();
}

/// <summary>
/// User analytics settings
/// </summary>
public class AnalyticsSettings
{
    public bool EnableUserAnalytics { get; set; } = true;
    public bool EnableBehaviorTracking { get; set; } = true;
    public bool EnablePerformanceMetrics { get; set; } = true;
    public bool EnableEngagementMetrics { get; set; } = true;
    public bool EnableConversionTracking { get; set; } = false;
    public bool EnableRealTimeAnalytics { get; set; } = false;
    public TimeSpan AnalyticsRetentionPeriod { get; set; } = TimeSpan.FromDays(365);
    public bool EnableAnalyticsExport { get; set; } = true;
    public bool EnableCustomEvents { get; set; } = true;
    public int MaxCustomEventsPerUser { get; set; } = 1000;
    public bool EnableAnalyticsDashboard { get; set; } = true;
    public bool EnableAnalyticsAlerts { get; set; } = true;
    public bool AnonymizeAnalyticsData { get; set; } = true;
    public bool EnableGdprCompliantAnalytics { get; set; } = true;
    public string[] AnalyticsExemptRoles { get; set; } = { "Admin", "SuperAdmin" };
}

/// <summary>
/// System maintenance settings
/// </summary>
public class MaintenanceSettings
{
    public bool EnableMaintenanceMode { get; set; } = false;
    public string MaintenanceMessage { get; set; } = "System is under maintenance. Please try again later.";
    public DateTime? ScheduledMaintenanceStart { get; set; }
    public DateTime? ScheduledMaintenanceEnd { get; set; }
    public string[] MaintenanceExemptRoles { get; set; } = { "SuperAdmin" };
    public string[] MaintenanceExemptIps { get; set; } = Array.Empty<string>();
    public bool EnableAutomaticBackups { get; set; } = true;
    public TimeSpan BackupInterval { get; set; } = TimeSpan.FromDays(1);
    public int MaxBackupRetention { get; set; } = 30;
    public bool EnableDatabaseOptimization { get; set; } = true;
    public TimeSpan OptimizationInterval { get; set; } = TimeSpan.FromDays(7);
    public bool EnableLogCleanup { get; set; } = true;
    public TimeSpan LogRetentionPeriod { get; set; } = TimeSpan.FromDays(90);
    public bool EnableCacheWarmup { get; set; } = true;
    public TimeSpan CacheWarmupInterval { get; set; } = TimeSpan.FromHours(6);
}

/// <summary>
/// Compliance and legal settings
/// </summary>
public class ComplianceSettings
{
    public bool EnableGdprCompliance { get; set; } = true;
    public bool EnableCcpaCompliance { get; set; } = false;
    public bool EnableCoppaCompliance { get; set; } = false;
    public int MinimumAge { get; set; } = 13;
    public bool RequireAgeVerification { get; set; } = false;
    public bool EnableDataPortability { get; set; } = true;
    public bool EnableRightToBeDeleted { get; set; } = true;
    public bool EnableConsentManagement { get; set; } = true;
    public bool RequireExplicitConsent { get; set; } = true;
    public bool EnableCookieConsent { get; set; } = true;
    public bool EnableDataProcessingLog { get; set; } = true;
    public TimeSpan DataProcessingLogRetention { get; set; } = TimeSpan.FromDays(365 * 6); // 6 years
    public bool EnablePrivacyPolicyVersioning { get; set; } = true;
    public bool RequirePrivacyPolicyAcceptance { get; set; } = true;
    public bool EnableTermsOfServiceVersioning { get; set; } = true;
    public bool RequireTermsOfServiceAcceptance { get; set; } = true;
    public string[] ComplianceContactEmails { get; set; } = Array.Empty<string>();
    public string DataProtectionOfficerEmail { get; set; } = string.Empty;
}