using CommunityCar.Application.Common.Interfaces.Services.Dashboard.BackgroundJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace CommunityCar.Infrastructure.BackgroundJobs;

public class BackgroundJobService : IBackgroundJobService
{
    private readonly ILogger<BackgroundJobService> _logger;
    private readonly ConcurrentDictionary<string, JobInfo> _jobs;
    private static int _jobCounter = 0;

    public BackgroundJobService(ILogger<BackgroundJobService> logger)
    {
        _logger = logger;
        _jobs = new ConcurrentDictionary<string, JobInfo>();
    }

    public Task<string> EnqueueAsync<T>(string methodName, T parameters) where T : class
    {
        var jobId = GenerateJobId();
        var jobInfo = new JobInfo
        {
            Id = jobId,
            MethodName = methodName,
            Parameters = parameters,
            Status = JobStatus.Enqueued,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _jobs.TryAdd(jobId, jobInfo);
        _logger.LogInformation("Job {JobId} enqueued for method {MethodName}", jobId, methodName);

        // In a real implementation, this would be queued to a job processor
        _ = Task.Run(async () => await ProcessJobAsync(jobInfo));

        return Task.FromResult(jobId);
    }

    public Task<string> ScheduleAsync<T>(string methodName, T parameters, TimeSpan delay) where T : class
    {
        var scheduleAt = DateTimeOffset.UtcNow.Add(delay);
        return ScheduleAsync(methodName, parameters, scheduleAt);
    }

    public Task<string> ScheduleAsync<T>(string methodName, T parameters, DateTimeOffset scheduleAt) where T : class
    {
        var jobId = GenerateJobId();
        var jobInfo = new JobInfo
        {
            Id = jobId,
            MethodName = methodName,
            Parameters = parameters,
            Status = JobStatus.Scheduled,
            CreatedAt = DateTimeOffset.UtcNow,
            ScheduledAt = scheduleAt
        };

        _jobs.TryAdd(jobId, jobInfo);
        _logger.LogInformation("Job {JobId} scheduled for method {MethodName} at {ScheduledAt}", jobId, methodName, scheduleAt);

        // Schedule the job
        var delay = scheduleAt - DateTimeOffset.UtcNow;
        if (delay > TimeSpan.Zero)
        {
            _ = Task.Delay(delay).ContinueWith(async _ => await ProcessJobAsync(jobInfo));
        }
        else
        {
            _ = Task.Run(async () => await ProcessJobAsync(jobInfo));
        }

        return Task.FromResult(jobId);
    }

    public Task<string> RecurringAsync<T>(string jobId, string methodName, T parameters, string cronExpression) where T : class
    {
        var jobInfo = new JobInfo
        {
            Id = jobId,
            MethodName = methodName,
            Parameters = parameters,
            Status = JobStatus.Recurring,
            CreatedAt = DateTimeOffset.UtcNow,
            CronExpression = cronExpression
        };

        _jobs.AddOrUpdate(jobId, jobInfo, (key, existing) => jobInfo);
        _logger.LogInformation("Recurring job {JobId} registered for method {MethodName} with cron {CronExpression}", jobId, methodName, cronExpression);

        // In a real implementation, this would use a proper cron scheduler
        return Task.FromResult(jobId);
    }

    public Task<bool> DeleteAsync(string jobId)
    {
        var removed = _jobs.TryRemove(jobId, out var jobInfo);
        if (removed && jobInfo != null)
        {
            jobInfo.Status = JobStatus.Deleted;
            _logger.LogInformation("Job {JobId} deleted", jobId);
        }
        return Task.FromResult(removed);
    }

    public Task<bool> CancelAsync(string jobId)
    {
        if (_jobs.TryGetValue(jobId, out var jobInfo))
        {
            jobInfo.Status = JobStatus.Cancelled;
            _logger.LogInformation("Job {JobId} cancelled", jobId);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    private async Task ProcessJobAsync(JobInfo jobInfo)
    {
        try
        {
            if (jobInfo.Status == JobStatus.Cancelled || jobInfo.Status == JobStatus.Deleted)
            {
                return;
            }

            jobInfo.Status = JobStatus.Processing;
            jobInfo.StartedAt = DateTimeOffset.UtcNow;

            _logger.LogInformation("Processing job {JobId} for method {MethodName}", jobInfo.Id, jobInfo.MethodName);

            // Simulate job processing
            await Task.Delay(1000);

            jobInfo.Status = JobStatus.Completed;
            jobInfo.CompletedAt = DateTimeOffset.UtcNow;

            _logger.LogInformation("Job {JobId} completed successfully", jobInfo.Id);
        }
        catch (Exception ex)
        {
            jobInfo.Status = JobStatus.Failed;
            jobInfo.Error = ex.Message;
            jobInfo.CompletedAt = DateTimeOffset.UtcNow;

            _logger.LogError(ex, "Job {JobId} failed", jobInfo.Id);
        }
    }

    private static string GenerateJobId()
    {
        return $"job_{DateTimeOffset.UtcNow:yyyyMMddHHmmss}_{System.Threading.Interlocked.Increment(ref _jobCounter)}";
    }

    private class JobInfo
    {
        public string Id { get; set; } = string.Empty;
        public string MethodName { get; set; } = string.Empty;
        public object? Parameters { get; set; }
        public JobStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? ScheduledAt { get; set; }
        public DateTimeOffset? StartedAt { get; set; }
        public DateTimeOffset? CompletedAt { get; set; }
        public string? CronExpression { get; set; }
        public string? Error { get; set; }
    }

    private enum JobStatus
    {
        Enqueued,
        Scheduled,
        Processing,
        Completed,
        Failed,
        Cancelled,
        Deleted,
        Recurring
    }
}
