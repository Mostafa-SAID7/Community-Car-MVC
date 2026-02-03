using CommunityCar.Application.Services.Dashboard.BackgroundJobs;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.BackgroundJobs;
using CommunityCar.Application.Features.Dashboard.ViewModels.BackgroundJobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hangfire;
using Hangfire.Storage;

namespace CommunityCar.Web.Controllers.Dashboard.System;

[Route("{culture=en-US}/dashboard/system/background-jobs")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class BackgroundJobsController : Controller
{
    private readonly BackgroundJobSchedulerService _schedulerService;
    private readonly IBackgroundJobService _backgroundJobService;
    private readonly GamificationBackgroundJobService _gamificationService;
    private readonly MaintenanceBackgroundJobService _maintenanceService;
    private readonly FeedBackgroundJobService _feedService;
    private readonly EmailBackgroundJobService _emailService;
    private readonly ILogger<BackgroundJobsController> _logger;

    public BackgroundJobsController(
        BackgroundJobSchedulerService schedulerService,
        IBackgroundJobService backgroundJobService,
        GamificationBackgroundJobService gamificationService,
        MaintenanceBackgroundJobService maintenanceService,
        FeedBackgroundJobService feedService,
        EmailBackgroundJobService emailService,
        ILogger<BackgroundJobsController> logger)
    {
        _schedulerService = schedulerService;
        _backgroundJobService = backgroundJobService;
        _gamificationService = gamificationService;
        _maintenanceService = maintenanceService;
        _feedService = feedService;
        _emailService = emailService;
        _logger = logger;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        try
        {
            // Temporary: Create empty model when Hangfire is not enabled
            var model = new BackgroundJobsViewModel
            {
                JobStatistics = new JobStatistics(),
                RecurringJobs = new List<RecurringJobInfo>(),
                RecentJobs = new List<RecentJobInfo>()
            };

            return View(model);
            
            // Original code commented out until Hangfire is enabled
            /*
            var model = new BackgroundJobsViewModel
            {
                JobStatistics = GetJobStatistics(),
                RecurringJobs = GetRecurringJobs(),
                RecentJobs = GetRecentJobs()
            };

            return View(model);
            */
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading background jobs page");
            TempData["ErrorMessage"] = "Failed to load background jobs information.";
            return View(new BackgroundJobsViewModel());
        }
    }

    [HttpPost("run-maintenance")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RunMaintenance()
    {
        try
        {
            await _schedulerService.RunDailyMaintenanceAsync();
            TempData["SuccessMessage"] = "Maintenance tasks started successfully.";
            _logger.LogInformation("Manual maintenance tasks started by admin user");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting maintenance tasks");
            TempData["ErrorMessage"] = "Failed to start maintenance tasks.";
        }

        return RedirectToAction("Index");
    }

    [HttpPost("update-feeds")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateFeeds()
    {
        try
        {
            await _schedulerService.RunHourlyFeedUpdateAsync();
            TempData["SuccessMessage"] = "Feed update started successfully.";
            _logger.LogInformation("Manual feed update started by admin user");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting feed update");
            TempData["ErrorMessage"] = "Failed to start feed update.";
        }

        return RedirectToAction("Index");
    }

    [HttpPost("update-trending")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateTrending()
    {
        try
        {
            await _schedulerService.UpdateTrendingTopicsAsync();
            TempData["SuccessMessage"] = "Trending topics update started successfully.";
            _logger.LogInformation("Manual trending topics update started by admin user");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting trending topics update");
            TempData["ErrorMessage"] = "Failed to start trending topics update.";
        }

        return RedirectToAction("Index");
    }

    [HttpPost("process-gamification")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessGamification()
    {
        try
        {
            await _schedulerService.RunGamificationProcessingAsync();
            TempData["SuccessMessage"] = "Gamification processing started successfully.";
            _logger.LogInformation("Manual gamification processing started by admin user");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting gamification processing");
            TempData["ErrorMessage"] = "Failed to start gamification processing.";
        }

        return RedirectToAction("Index");
    }

    [HttpPost("send-email-digest")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendEmailDigest()
    {
        try
        {
            await _schedulerService.SendDailyEmailDigestAsync();
            TempData["SuccessMessage"] = "Email digest sending started successfully.";
            _logger.LogInformation("Manual email digest started by admin user");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting email digest");
            TempData["ErrorMessage"] = "Failed to start email digest.";
        }

        return RedirectToAction("Index");
    }

    [HttpGet("statistics")]
    public IActionResult GetStatistics()
    {
        try
        {
            var statistics = GetJobStatistics();
            return Json(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting job statistics");
            return Json(new { error = "Failed to get statistics" });
        }
    }

    [HttpGet("recurring")]
    public IActionResult RecurringJobs()
    {
        try
        {
            var model = new RecurringJobsViewModel
            {
                Jobs = GetRecurringJobs()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading recurring jobs");
            TempData["ErrorMessage"] = "Failed to load recurring jobs.";
            return View(new RecurringJobsViewModel());
        }
    }

    [HttpPost("recurring/{jobId}/trigger")]
    [ValidateAntiForgeryToken]
    public IActionResult TriggerRecurringJob(string jobId)
    {
        try
        {
            RecurringJob.TriggerJob(jobId);
            TempData["SuccessMessage"] = $"Job '{jobId}' triggered successfully.";
            _logger.LogInformation("Recurring job {JobId} triggered by admin user", jobId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error triggering recurring job {JobId}", jobId);
            TempData["ErrorMessage"] = $"Failed to trigger job '{jobId}'.";
        }

        return RedirectToAction("RecurringJobs");
    }

    private JobStatistics GetJobStatistics()
    {
        try
        {
            // Check if Hangfire is enabled
            if (JobStorage.Current == null)
            {
                _logger.LogWarning("Hangfire is not initialized - returning empty job statistics");
                return new JobStatistics();
            }

            using var connection = JobStorage.Current.GetConnection();
            var monitoring = JobStorage.Current.GetMonitoringApi();

            return new JobStatistics
            {
                EnqueuedCount = monitoring.EnqueuedCount("default"),
                ProcessingCount = monitoring.ProcessingCount(),
                SucceededCount = monitoring.SucceededListCount(),
                FailedCount = monitoring.FailedCount(),
                ScheduledCount = monitoring.ScheduledCount(),
                ServersCount = monitoring.Servers().Count
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting job statistics");
            return new JobStatistics();
        }
    }

    private List<RecurringJobInfo> GetRecurringJobs()
    {
        try
        {
            // Check if Hangfire is enabled
            if (JobStorage.Current == null)
            {
                _logger.LogWarning("Hangfire is not initialized - returning empty recurring jobs list");
                return new List<RecurringJobInfo>();
            }

            using var connection = JobStorage.Current.GetConnection();
            var recurringJobs = connection.GetRecurringJobs();

            return recurringJobs.Select(job => new RecurringJobInfo
            {
                Id = job.Id,
                Cron = job.Cron,
                NextExecution = job.NextExecution,
                LastExecution = job.LastExecution,
                LastJobState = job.LastJobState,
                Error = job.Error
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recurring jobs");
            return new List<RecurringJobInfo>();
        }
    }

    private List<RecentJobInfo> GetRecentJobs()
    {
        try
        {
            // Check if Hangfire is enabled
            if (JobStorage.Current == null)
            {
                _logger.LogWarning("Hangfire is not initialized - returning empty recent jobs list");
                return new List<RecentJobInfo>();
            }

            var monitoring = JobStorage.Current.GetMonitoringApi();
            var succeededJobs = monitoring.SucceededJobs(0, 10);
            var failedJobs = monitoring.FailedJobs(0, 10);

            var recentJobs = new List<RecentJobInfo>();

            foreach (var job in succeededJobs)
            {
                recentJobs.Add(new RecentJobInfo
                {
                    Id = job.Key,
                    Method = job.Value.Job?.Method?.Name ?? "Unknown",
                    State = "Succeeded",
                    CreatedAt = job.Value.SucceededAt,
                    Duration = job.Value.TotalDuration.HasValue ? TimeSpan.FromMilliseconds(job.Value.TotalDuration.Value) : null
                });
            }

            foreach (var job in failedJobs)
            {
                recentJobs.Add(new RecentJobInfo
                {
                    Id = job.Key,
                    Method = job.Value.Job?.Method?.Name ?? "Unknown",
                    State = "Failed",
                    CreatedAt = job.Value.FailedAt,
                    Duration = null, // Failed jobs don't carry duration in DTO usually
                    Error = job.Value.ExceptionDetails
                });
            }

            return recentJobs.OrderByDescending(j => j.CreatedAt).Take(20).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent jobs");
            return new List<RecentJobInfo>();
        }
    }
}