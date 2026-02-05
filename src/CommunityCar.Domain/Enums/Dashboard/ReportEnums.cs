namespace CommunityCar.Domain.Enums.Dashboard;

public enum ReportType
{
    UserAnalytics = 1,
    ContentAnalytics = 2,
    SystemPerformance = 3,
    SecurityAudit = 4,
    FinancialReport = 5,
    TrafficAnalytics = 6,
    ErrorReport = 7,
    MaintenanceReport = 8,
    BackgroundJobsReport = 9,
    CacheReport = 10,
    LocalizationReport = 11,
    SEOReport = 12,
    MonitoringReport = 13,
    AuditReport = 14,
    SoftDeleteReport = 15,
    CustomReport = 99
}

public enum ReportStatus
{
    Pending = 1,
    Generating = 2,
    Generated = 3,
    Failed = 4,
    Scheduled = 5,
    Expired = 6,
    Cancelled = 7
}

public enum ReportFormat
{
    PDF = 1,
    Excel = 2,
    CSV = 3,
    JSON = 4,
    XML = 5,
    HTML = 6
}

public enum ScheduleFrequency
{
    Once = 1,
    Daily = 2,
    Weekly = 3,
    Monthly = 4,
    Quarterly = 5,
    Yearly = 6,
    Custom = 99
}