using CommunityCar.Application.Common.Interfaces.Services.BackgroundJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CommunityCar.Infrastructure.BackgroundJobs;

public class ScheduledJobsHostedService : BackgroundService
{
    private readonly ILogger<ScheduledJobsHostedService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly Timer _profileStatsTimer;
    private readonly Timer _feedAggregationTimer;
    private readonly Timer _notificationTimer;
    private readonly Timer _dataCleanupTimer;
    private readonly Timer _analyticsTimer;

    public ScheduledJobsHostedService(
        ILogger<ScheduledJobsHostedService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;

        // Set up recurring timers
        _profileStatsTimer = new Timer(ProcessProfileStatsUpdate, null, TimeSpan.Zero, TimeSpan.FromHours(1));
        _feedAggregationTimer = new Timer(ProcessFeedAggregation, null, TimeSpan.Zero, TimeSpan.FromMinutes(15));
        _notificationTimer = new Timer(ProcessNotifications, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        _dataCleanupTimer = new Timer(ProcessDataCleanup, null, TimeSpan.FromHours(1), TimeSpan.FromHours(24));
        _analyticsTimer = new Timer(ProcessAnalytics, null, TimeSpan.FromMinutes(30), TimeSpan.FromHours(6));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Scheduled Jobs Hosted Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Keep the service running
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ScheduledJobsHostedService");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        _logger.LogInformation("Scheduled Jobs Hosted Service stopped");
    }

    private async void ProcessProfileStatsUpdate(object? state)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var jobProcessor = scope.ServiceProvider.GetRequiredService<IJobProcessor>();
            
            _logger.LogInformation("Starting scheduled profile statistics update");
            
            // In a real implementation, you would get a list of users that need updates
            // For now, we'll just log that the job is running
            _logger.LogInformation("Profile statistics update job completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in scheduled profile statistics update");
        }
    }

    private async void ProcessFeedAggregation(object? state)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var jobProcessor = scope.ServiceProvider.GetRequiredService<IJobProcessor>();
            
            _logger.LogInformation("Starting scheduled feed content aggregation");
            await jobProcessor.ProcessFeedContentAggregationAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in scheduled feed content aggregation");
        }
    }

    private async void ProcessNotifications(object? state)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var jobProcessor = scope.ServiceProvider.GetRequiredService<IJobProcessor>();
            
            _logger.LogInformation("Starting scheduled notification processing");
            await jobProcessor.ProcessNotificationBatchAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in scheduled notification processing");
        }
    }

    private async void ProcessDataCleanup(object? state)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var jobProcessor = scope.ServiceProvider.GetRequiredService<IJobProcessor>();
            
            _logger.LogInformation("Starting scheduled data cleanup");
            await jobProcessor.ProcessDataCleanupAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in scheduled data cleanup");
        }
    }

    private async void ProcessAnalytics(object? state)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var jobProcessor = scope.ServiceProvider.GetRequiredService<IJobProcessor>();
            
            _logger.LogInformation("Starting scheduled analytics aggregation");
            await jobProcessor.ProcessAnalyticsAggregationAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in scheduled analytics aggregation");
        }
    }

    public override void Dispose()
    {
        _profileStatsTimer?.Dispose();
        _feedAggregationTimer?.Dispose();
        _notificationTimer?.Dispose();
        _dataCleanupTimer?.Dispose();
        _analyticsTimer?.Dispose();
        base.Dispose();
    }
}
