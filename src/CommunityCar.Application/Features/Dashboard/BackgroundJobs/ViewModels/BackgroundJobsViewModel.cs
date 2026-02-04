namespace CommunityCar.Application.Features.Dashboard.BackgroundJobs.ViewModels;

public class BackgroundJobsViewModel
{
    public List<RecurringJobInfo> RecurringJobs { get; set; } = new();
    public List<RecentJobInfo> RecentJobs { get; set; } = new();
    public JobStatistics Statistics { get; set; } = new();
    public JobStatistics JobStatistics { get; set; } = new();
    public int TotalJobs { get; set; }
    public int RunningJobs { get; set; }
    public int FailedJobs { get; set; }
    public int CompletedJobs { get; set; }
    public DateTime LastUpdated { get; set; }
}