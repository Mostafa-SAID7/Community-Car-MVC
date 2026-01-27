namespace CommunityCar.Infrastructure.Configuration;

public class BackgroundJobSettings
{
    public const string SectionName = "BackgroundJobs";

    public bool EnableScheduledJobs { get; set; } = true;
    public JobScheduleSettings Schedules { get; set; } = new();
    public int MaxRetryAttempts { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromMinutes(5);
}

public class JobScheduleSettings
{
    public TimeSpan ProfileStatsUpdateInterval { get; set; } = TimeSpan.FromHours(1);
    public TimeSpan FeedAggregationInterval { get; set; } = TimeSpan.FromMinutes(15);
    public TimeSpan NotificationProcessingInterval { get; set; } = TimeSpan.FromMinutes(5);
    public TimeSpan DataCleanupInterval { get; set; } = TimeSpan.FromHours(24);
    public TimeSpan AnalyticsAggregationInterval { get; set; } = TimeSpan.FromHours(6);
    public TimeSpan EmailBatchInterval { get; set; } = TimeSpan.FromMinutes(10);
    public TimeSpan ContentModerationInterval { get; set; } = TimeSpan.FromMinutes(2);
}
