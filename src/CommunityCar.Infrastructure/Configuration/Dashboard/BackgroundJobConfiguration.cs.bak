using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Configuration;


public static class BackgroundJobConfiguration
{
    public static IServiceCollection AddBackgroundJobs(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        // Add Hangfire services
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true
            }));

        // Add the processing server as IHostedService
        services.AddHangfireServer(options =>
        {
            options.WorkerCount = Environment.ProcessorCount * 2;
            options.Queues = new[] { "critical", "default", "background" };
            options.ServerTimeout = TimeSpan.FromMinutes(4);
            options.SchedulePollingInterval = TimeSpan.FromSeconds(15);
        });

        return services;
    }

    public static void ConfigureRecurringJobs(IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<CommunityCar.Application.Services.Dashboard.BackgroundJobs.BackgroundJobSchedulerService>>();
        
        try
        {
            logger.LogInformation("Configuring recurring background jobs");

            // Daily maintenance at 2 AM
            RecurringJob.AddOrUpdate<CommunityCar.Application.Services.Dashboard.BackgroundJobs.BackgroundJobSchedulerService>(
                "daily-maintenance",
                service => service.RunDailyMaintenanceAsync(),
                "0 2 * * *", // Daily at 2 AM
                TimeZoneInfo.Utc);

            // Hourly feed updates
            RecurringJob.AddOrUpdate<CommunityCar.Application.Services.Dashboard.BackgroundJobs.BackgroundJobSchedulerService>(
                "hourly-feed-update",
                service => service.RunHourlyFeedUpdateAsync(),
                "0 * * * *", // Every hour
                TimeZoneInfo.Utc);

            // Trending topics every 15 minutes
            RecurringJob.AddOrUpdate<CommunityCar.Application.Services.Dashboard.BackgroundJobs.BackgroundJobSchedulerService>(
                "trending-topics-update",
                service => service.UpdateTrendingTopicsAsync(),
                "*/15 * * * *", // Every 15 minutes
                TimeZoneInfo.Utc);

            // Gamification processing every 30 minutes
            RecurringJob.AddOrUpdate<CommunityCar.Application.Services.Dashboard.BackgroundJobs.BackgroundJobSchedulerService>(
                "gamification-processing",
                service => service.RunGamificationProcessingAsync(),
                "*/30 * * * *", // Every 30 minutes
                TimeZoneInfo.Utc);

            // Weekly cleanup on Sunday at 3 AM
            RecurringJob.AddOrUpdate<CommunityCar.Application.Services.Dashboard.BackgroundJobs.BackgroundJobSchedulerService>(
                "weekly-cleanup",
                service => service.RunWeeklyCleanupAsync(),
                "0 3 * * 0", // Sunday at 3 AM
                TimeZoneInfo.Utc);

            // Daily email digest at 8 AM
            RecurringJob.AddOrUpdate<CommunityCar.Application.Services.Dashboard.BackgroundJobs.BackgroundJobSchedulerService>(
                "daily-email-digest",
                service => service.SendDailyEmailDigestAsync(),
                "0 8 * * *", // Daily at 8 AM
                TimeZoneInfo.Utc);

            // Cache warmup every 4 hours
            RecurringJob.AddOrUpdate<CommunityCar.Application.Services.Dashboard.Caching.CacheWarmupService>(
                "cache-warmup",
                service => service.WarmupAllCacheAsync(),
                "0 */4 * * *", // Every 4 hours
                TimeZoneInfo.Utc);

            logger.LogInformation("Recurring background jobs configured successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to configure recurring background jobs");
            throw;
        }
    }
}
