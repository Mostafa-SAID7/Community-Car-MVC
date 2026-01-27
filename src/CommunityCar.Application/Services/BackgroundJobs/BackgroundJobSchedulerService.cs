using CommunityCar.Application.Common.Interfaces.Services.BackgroundJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace CommunityCar.Application.Services.BackgroundJobs;

/// <summary>
/// Centralized background job scheduler service
/// </summary>
public class BackgroundJobSchedulerService
{
    private readonly IBackgroundJobService _backgroundJobService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackgroundJobSchedulerService> _logger;

    public BackgroundJobSchedulerService(
        IBackgroundJobService backgroundJobService,
        IServiceProvider serviceProvider,
        ILogger<BackgroundJobSchedulerService> logger)
    {
        _backgroundJobService = backgroundJobService;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Schedule all recurring background jobs
    /// </summary>
    public async Task ScheduleRecurringJobsAsync()
    {
        try
        {
            _logger.LogInformation("Scheduling recurring background jobs");

            // Daily maintenance jobs (run at 2 AM)
            await _backgroundJobService.RecurringAsync(
                "daily-maintenance",
                nameof(RunDailyMaintenanceAsync),
                new { },
                "0 2 * * *" // Daily at 2 AM
            );

            // Hourly feed updates
            await _backgroundJobService.RecurringAsync(
                "hourly-feed-update",
                nameof(RunHourlyFeedUpdateAsync),
                new { },
                "0 * * * *" // Every hour
            );

            // Every 15 minutes - trending topics update
            await _backgroundJobService.RecurringAsync(
                "trending-topics-update",
                nameof(UpdateTrendingTopicsAsync),
                new { },
                "*/15 * * * *" // Every 15 minutes
            );

            // Every 30 minutes - gamification processing
            await _backgroundJobService.RecurringAsync(
                "gamification-processing",
                nameof(RunGamificationProcessingAsync),
                new { },
                "*/30 * * * *" // Every 30 minutes
            );

            // Weekly cleanup (run on Sunday at 3 AM)
            await _backgroundJobService.RecurringAsync(
                "weekly-cleanup",
                nameof(RunWeeklyCleanupAsync),
                new { },
                "0 3 * * 0" // Sunday at 3 AM
            );

            // Daily email digest (run at 8 AM)
            await _backgroundJobService.RecurringAsync(
                "daily-email-digest",
                nameof(SendDailyEmailDigestAsync),
                new { },
                "0 8 * * *" // Daily at 8 AM
            );

            _logger.LogInformation("Recurring background jobs scheduled successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to schedule recurring background jobs");
            throw;
        }
    }

    /// <summary>
    /// Run daily maintenance tasks
    /// </summary>
    public async Task RunDailyMaintenanceAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var maintenanceService = scope.ServiceProvider.GetRequiredService<MaintenanceBackgroundJobService>();
        var gamificationService = scope.ServiceProvider.GetRequiredService<GamificationBackgroundJobService>();

        try
        {
            _logger.LogInformation("Starting daily maintenance tasks");

            // System maintenance
            await maintenanceService.CleanupOldErrorLogsAsync();
            await maintenanceService.CleanupOldUserActivitiesAsync();
            await maintenanceService.UpdateDatabaseStatisticsAsync();
            await maintenanceService.GenerateSystemHealthReportAsync();
            await maintenanceService.ValidateDataIntegrityAsync();

            // Gamification maintenance
            await gamificationService.ProcessDailyTasksAsync();

            _logger.LogInformation("Daily maintenance tasks completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to run daily maintenance tasks");
            throw;
        }
    }

    /// <summary>
    /// Run hourly feed updates
    /// </summary>
    public async Task RunHourlyFeedUpdateAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var feedService = scope.ServiceProvider.GetRequiredService<FeedBackgroundJobService>();

        try
        {
            _logger.LogInformation("Starting hourly feed update");

            // Get active users from the last hour
            var activeUserIds = await GetActiveUserIdsAsync(TimeSpan.FromHours(1));
            
            // Pre-generate feeds for active users
            await feedService.PreGeneratePersonalizedFeedsAsync(activeUserIds);
            
            // Update popular content
            await feedService.UpdatePopularContentAsync();
            
            // Clean up expired stories
            await feedService.CleanupExpiredStoriesAsync();

            _logger.LogInformation("Hourly feed update completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to run hourly feed update");
            throw;
        }
    }

    /// <summary>
    /// Update trending topics
    /// </summary>
    public async Task UpdateTrendingTopicsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var feedService = scope.ServiceProvider.GetRequiredService<FeedBackgroundJobService>();

        try
        {
            _logger.LogInformation("Updating trending topics");
            await feedService.UpdateTrendingTopicsAsync();
            _logger.LogInformation("Trending topics updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update trending topics");
            throw;
        }
    }

    /// <summary>
    /// Run gamification processing
    /// </summary>
    public async Task RunGamificationProcessingAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var gamificationService = scope.ServiceProvider.GetRequiredService<GamificationBackgroundJobService>();

        try
        {
            _logger.LogInformation("Starting gamification processing");

            // Update leaderboards
            await gamificationService.UpdateLeaderboardsAsync();

            // Get recent user actions and process them
            var recentActions = await GetRecentUserActionsAsync(TimeSpan.FromMinutes(30));
            if (recentActions.Any())
            {
                await gamificationService.BatchProcessUserActionsAsync(recentActions);
            }

            _logger.LogInformation("Gamification processing completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to run gamification processing");
            throw;
        }
    }

    /// <summary>
    /// Run weekly cleanup tasks
    /// </summary>
    public async Task RunWeeklyCleanupAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var maintenanceService = scope.ServiceProvider.GetRequiredService<MaintenanceBackgroundJobService>();

        try
        {
            _logger.LogInformation("Starting weekly cleanup tasks");

            // Optimize database
            await maintenanceService.OptimizeDatabaseIndexesAsync();
            
            // Backup critical data
            await maintenanceService.BackupCriticalDataAsync();
            
            // Clean up cache
            await maintenanceService.CleanupExpiredCacheAsync();

            _logger.LogInformation("Weekly cleanup tasks completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to run weekly cleanup tasks");
            throw;
        }
    }

    /// <summary>
    /// Send daily email digest
    /// </summary>
    public async Task SendDailyEmailDigestAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var emailService = scope.ServiceProvider.GetRequiredService<EmailBackgroundJobService>();

        try
        {
            _logger.LogInformation("Starting daily email digest");

            // Get users who opted in for daily digest
            var digestUsers = await GetDailyDigestUsersAsync();
            
            var emailTasks = digestUsers.Select(async user =>
            {
                try
                {
                    var notifications = await GetUserNotificationsAsync(user.UserId, TimeSpan.FromDays(1));
                    if (notifications.Any())
                    {
                        await emailService.SendNotificationDigestAsync(user.Email, user.UserName, notifications);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send daily digest to user {UserId}", user.UserId);
                }
            });

            await Task.WhenAll(emailTasks);
            _logger.LogInformation("Daily email digest completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send daily email digest");
            throw;
        }
    }

    /// <summary>
    /// Schedule immediate job
    /// </summary>
    public async Task<string> ScheduleImmediateJobAsync<T>(string methodName, T parameters) where T : class
    {
        return await _backgroundJobService.EnqueueAsync(methodName, parameters);
    }

    /// <summary>
    /// Schedule delayed job
    /// </summary>
    public async Task<string> ScheduleDelayedJobAsync<T>(string methodName, T parameters, TimeSpan delay) where T : class
    {
        return await _backgroundJobService.ScheduleAsync(methodName, parameters, delay);
    }

    /// <summary>
    /// Cancel job
    /// </summary>
    public async Task<bool> CancelJobAsync(string jobId)
    {
        return await _backgroundJobService.CancelAsync(jobId);
    }

    #region Helper Methods

    private async Task<List<Guid>> GetActiveUserIdsAsync(TimeSpan timeSpan)
    {
        // TODO: Implement logic to get active user IDs from the database
        // This would query user activities within the specified timespan
        await Task.Delay(1); // Placeholder
        return new List<Guid>();
    }

    private async Task<List<(Guid UserId, string ActionType, Dictionary<string, object>? ActionData)>> GetRecentUserActionsAsync(TimeSpan timeSpan)
    {
        // TODO: Implement logic to get recent user actions from the database
        await Task.Delay(1); // Placeholder
        return new List<(Guid, string, Dictionary<string, object>?)>();
    }

    private async Task<List<(Guid UserId, string Email, string UserName)>> GetDailyDigestUsersAsync()
    {
        // TODO: Implement logic to get users who opted in for daily digest
        await Task.Delay(1); // Placeholder
        return new List<(Guid, string, string)>();
    }

    private async Task<List<string>> GetUserNotificationsAsync(Guid userId, TimeSpan timeSpan)
    {
        // TODO: Implement logic to get user notifications from the specified timespan
        await Task.Delay(1); // Placeholder
        return new List<string>();
    }

    #endregion
}


