namespace CommunityCar.Application.Features.Dashboard.ViewModels;

public class OverviewVM
{
    public string TimeRange { get; set; } = "Last 30 days";
    public StatsVM Stats { get; set; } = new();
    public List<RecentActivityVM> RecentActivity { get; set; } = new();
    public List<TopContentVM> TopContent { get; set; } = new();
    public List<ActiveUserVM> ActiveUsers { get; set; } = new();
    public List<ChartDataVM> UserGrowthChart { get; set; } = new();
    public List<ChartDataVM> ContentChart { get; set; } = new();
    public List<ChartDataVM> EngagementChart { get; set; } = new();
    public SystemHealthVM SystemHealth { get; set; } = new();
    
    // Date range properties
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class StatsVM
{
    public int TotalUsers { get; set; }
    public int TotalPosts { get; set; }
    public int TotalComments { get; set; }
    public int TotalQuestions { get; set; }
    public int TotalAnswers { get; set; }
    public int TotalReviews { get; set; }
    public int TotalStories { get; set; }
    public int TotalNews { get; set; }
    public int TotalInteractions { get; set; }
    public int ActiveUsersToday { get; set; }
    public int ActiveUsersThisWeek { get; set; }
    public int ActiveUsersThisMonth { get; set; }
    public decimal GrowthRate { get; set; }
    public decimal EngagementRate { get; set; }
    public DateTime LastUpdated { get; set; }

    // Properties for Quick Stats widgets
    public string Title { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public decimal ChangePercentage { get; set; }
    public bool IsPositiveChange { get; set; }
}

public class RecentActivityVM
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}

public class TopContentVM
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public int Views { get; set; }
    public int Likes { get; set; }
    public int Comments { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ActiveUserVM
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public DateTime LastActivity { get; set; }
    public string LastActivityText { get; set; } = string.Empty;
    public int PostsCount { get; set; }
    public int CommentsCount { get; set; }
    public bool IsOnline { get; set; }
}

public class ChartDataVM
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public DateTime Date { get; set; }
    public string Color { get; set; } = string.Empty;
}

public class SystemHealthVM
{
    public DateTime CheckTime { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public double ResponseTime { get; set; }
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public double DiskUsage { get; set; }
    public int ActiveConnections { get; set; }
    public double Uptime { get; set; }
    public string Version { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public bool IsHealthy { get; set; }
    public int ErrorCount { get; set; }
    public int WarningCount { get; set; }
    public List<string> Issues { get; set; } = new();
    public DateTime LastCheck { get; set; }
}

public class UserAnalyticsVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int LoginCount { get; set; }
    public int PostsCreated { get; set; }
    public int QuestionsAsked { get; set; }
    public int AnswersGiven { get; set; }
    public int ReviewsWritten { get; set; }
    public int StoriesShared { get; set; }
    public int InteractionsCount { get; set; }
    public TimeSpan TimeSpentOnSite { get; set; }
    public int PageViews { get; set; }
    public string MostVisitedSection { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public string BrowserType { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;

    public int NewUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int ReturnUsers { get; set; }
    public decimal RetentionRate { get; set; }
    public decimal ChurnRate { get; set; }
    public TimeSpan AverageSessionDuration { get; set; }
    public List<ChartDataVM> UserGrowthData { get; set; } = new();
    public List<ChartDataVM> ActivityData { get; set; } = new();
}

public class ContentAnalyticsVM
{
    public Guid ContentId { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string ContentTitle { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int Views { get; set; }
    public int Likes { get; set; }
    public int Comments { get; set; }
    public int Shares { get; set; }
    public int Bookmarks { get; set; }
    public decimal EngagementRate { get; set; }
    public TimeSpan AverageViewTime { get; set; }
    public int UniqueViewers { get; set; }
    public string TopReferrer { get; set; } = string.Empty;
    public string TopKeyword { get; set; } = string.Empty;

    public int PostsCreated { get; set; }
    public int CommentsCreated { get; set; }
    public int QuestionsCreated { get; set; }
    public int AnswersCreated { get; set; }
    public int ReviewsCreated { get; set; }
    public int StoriesCreated { get; set; }
    public int NewsCreated { get; set; }
    public int TotalLikes { get; set; }
    public int TotalShares { get; set; }
    public int TotalViews { get; set; }
    public List<ChartDataVM> ContentCreationData { get; set; } = new();
    public List<ChartDataVM> EngagementData { get; set; } = new();
}

public class TrafficAnalyticsVM
{
    public int PageViews { get; set; }
    public int UniquePageViews { get; set; }
    public decimal BounceRate { get; set; }
    public TimeSpan AverageSessionDuration { get; set; }
    public List<ChartDataVM> TrafficData { get; set; } = new();
    public List<TopPageVM> TopPages { get; set; } = new();
}

public class TopPageVM
{
    public string Path { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Views { get; set; }
    public int UniqueViews { get; set; }
    public decimal BounceRate { get; set; }
}

public class UserManagementVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string PerformedByName { get; set; } = string.Empty;
    public DateTime ActionDate { get; set; }
    public string ActionDateFormatted { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsBlocked { get; set; }
    public DateTime LastLogin { get; set; }
    public int LoginCount { get; set; }
    public int PostCount { get; set; }
    public int CommentCount { get; set; }
    public int ReportCount { get; set; }
    public string? Notes { get; set; }
    public bool IsReversible { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsExpired { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ContentModerationVM
{
    public List<ModerationItemVM> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PendingCount { get; set; }
    public int ApprovedCount { get; set; }
    public int RejectedCount { get; set; }
}

public class ModerationItemVM
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public int ReportCount { get; set; }
    public List<string> ReportReasons { get; set; } = new();
}

public class SystemAlertVM
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
    public string? ActionUrl { get; set; }
}

public class SystemReportVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime GeneratedDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string GeneratedByName { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public int DownloadCount { get; set; }
    public long? FileSize { get; set; }
    public string FileSizeFormatted { get; set; } = string.Empty;
    public string? FileUrl { get; set; }
}

public class SettingsVM
{
    public string Key { get; set; } = string.Empty;
    public string SettingKey { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class AnalyticsVM
{
    public string ContentType { get; set; } = "All";
    public UserAnalyticsVM UserAnalytics { get; set; } = new();
    public ContentAnalyticsVM ContentAnalytics { get; set; } = new();
    public TrafficAnalyticsVM TrafficAnalytics { get; set; } = new();
    public List<ChartDataVM> OverviewCharts { get; set; } = new();
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
}

public class ModerateContentVM
{
    public Guid ContentId { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // "approve", "reject", "flag"
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public bool NotifyUser { get; set; } = true;
}

public class CreateReportVM
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Format { get; set; } = "PDF";
    public bool IsPublic { get; set; }
    public List<string> IncludeMetrics { get; set; } = new();
}

public class ReportGenerationVM
{
    public string ReportType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Format { get; set; } = "PDF";
    public List<string> Metrics { get; set; } = new();
    public Dictionary<string, object> Parameters { get; set; } = new();
}

public class ReportScheduleVM
{
    public string Name { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public string Schedule { get; set; } = string.Empty; // "daily", "weekly", "monthly"
    public string Format { get; set; } = "PDF";
    public List<string> Recipients { get; set; } = new();
    public bool IsActive { get; set; } = true;
}


// Error Management ViewModels (converted from DTOs)
public class ErrorStatsVM
{
    public DateTime Date { get; set; }
    public int TotalErrors { get; set; }
    public int CriticalErrors { get; set; }
    public int WarningErrors { get; set; }
    public int InfoErrors { get; set; }
    public int ResolvedErrors { get; set; }
    public int UnresolvedErrors { get; set; }
    public string? MostCommonError { get; set; }
    public int MostCommonErrorCount { get; set; }
    public Dictionary<string, int> ErrorsByCategory { get; set; } = new();
    public Dictionary<string, int> ErrorsBySeverity { get; set; } = new();
}

public class ErrorBoundaryVM
{
    public string Id { get; set; } = string.Empty;
    public string BoundaryName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public bool IsRecovered { get; set; }
    public string? RecoveryAction { get; set; }
    public DateTime? RecoveredAt { get; set; }
    public int FailureCount { get; set; }
}