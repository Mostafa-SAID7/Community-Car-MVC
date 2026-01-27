using CommunityCar.Application.Common.Interfaces.Services.BackgroundJobs;
using Hangfire;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace CommunityCar.Infrastructure.Services.BackgroundJobs;

/// <summary>
/// Hangfire implementation of background job service
/// </summary>
public class HangfireBackgroundJobService : IBackgroundJobService
{
    private readonly ILogger<HangfireBackgroundJobService> _logger;

    public HangfireBackgroundJobService(ILogger<HangfireBackgroundJobService> logger)
    {
        _logger = logger;
    }

    public string Enqueue(Expression<Func<Task>> methodCall)
    {
        try
        {
            var jobId = BackgroundJob.Enqueue(methodCall);
            _logger.LogDebug("Background job enqueued: {JobId}", jobId);
            return jobId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enqueuing background job");
            throw;
        }
    }

    public string Enqueue<T>(Expression<Func<T, Task>> methodCall)
    {
        try
        {
            var jobId = BackgroundJob.Enqueue<T>(methodCall);
            _logger.LogDebug("Background job enqueued: {JobId}", jobId);
            return jobId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enqueuing background job");
            throw;
        }
    }

    public string Schedule(Expression<Func<Task>> methodCall, TimeSpan delay)
    {
        try
        {
            var jobId = BackgroundJob.Schedule(methodCall, delay);
            _logger.LogDebug("Background job scheduled: {JobId} (Delay: {Delay})", jobId, delay);
            return jobId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scheduling background job with delay: {Delay}", delay);
            throw;
        }
    }

    public string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay)
    {
        try
        {
            var jobId = BackgroundJob.Schedule<T>(methodCall, delay);
            _logger.LogDebug("Background job scheduled: {JobId} (Delay: {Delay})", jobId, delay);
            return jobId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scheduling background job with delay: {Delay}", delay);
            throw;
        }
    }

    public string Schedule(Expression<Func<Task>> methodCall, DateTimeOffset enqueueAt)
    {
        try
        {
            var jobId = BackgroundJob.Schedule(methodCall, enqueueAt);
            _logger.LogDebug("Background job scheduled: {JobId} (EnqueueAt: {EnqueueAt})", jobId, enqueueAt);
            return jobId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scheduling background job at: {EnqueueAt}", enqueueAt);
            throw;
        }
    }

    public string Schedule<T>(Expression<Func<T, Task>> methodCall, DateTimeOffset enqueueAt)
    {
        try
        {
            var jobId = BackgroundJob.Schedule<T>(methodCall, enqueueAt);
            _logger.LogDebug("Background job scheduled: {JobId} (EnqueueAt: {EnqueueAt})", jobId, enqueueAt);
            return jobId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scheduling background job at: {EnqueueAt}", enqueueAt);
            throw;
        }
    }

    public void AddOrUpdate(string recurringJobId, Expression<Func<Task>> methodCall, string cronExpression, TimeZoneInfo? timeZone = null)
    {
        try
        {
            RecurringJob.AddOrUpdate(recurringJobId, methodCall, cronExpression, timeZone ?? TimeZoneInfo.Utc);
            _logger.LogDebug("Recurring job added/updated: {JobId} (Cron: {CronExpression})", recurringJobId, cronExpression);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding/updating recurring job: {JobId}", recurringJobId);
            throw;
        }
    }

    public void AddOrUpdate<T>(string recurringJobId, Expression<Func<T, Task>> methodCall, string cronExpression, TimeZoneInfo? timeZone = null)
    {
        try
        {
            RecurringJob.AddOrUpdate<T>(recurringJobId, methodCall, cronExpression, timeZone ?? TimeZoneInfo.Utc);
            _logger.LogDebug("Recurring job added/updated: {JobId} (Cron: {CronExpression})", recurringJobId, cronExpression);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding/updating recurring job: {JobId}", recurringJobId);
            throw;
        }
    }

    public void RemoveIfExists(string recurringJobId)
    {
        try
        {
            RecurringJob.RemoveIfExists(recurringJobId);
            _logger.LogDebug("Recurring job removed: {JobId}", recurringJobId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing recurring job: {JobId}", recurringJobId);
            throw;
        }
    }

    public bool Delete(string jobId)
    {
        try
        {
            var result = BackgroundJob.Delete(jobId);
            _logger.LogDebug("Background job deleted: {JobId} (Success: {Success})", jobId, result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting background job: {JobId}", jobId);
            return false;
        }
    }

    public bool Requeue(string jobId)
    {
        try
        {
            var result = BackgroundJob.Requeue(jobId);
            _logger.LogDebug("Background job requeued: {JobId} (Success: {Success})", jobId, result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error requeuing background job: {JobId}", jobId);
            return false;
        }
    }
}
