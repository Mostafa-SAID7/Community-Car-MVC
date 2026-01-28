namespace CommunityCar.Web.Models.Dashboard.System.BackgroundJobs;

public class BackgroundJobsViewModel
{
    public JobStatistics JobStatistics { get; set; } = new();
    public List<RecurringJobInfo> RecurringJobs { get; set; } = new();
    public List<RecentJobInfo> RecentJobs { get; set; } = new();
}