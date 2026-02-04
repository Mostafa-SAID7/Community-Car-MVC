namespace CommunityCar.Application.Features.Dashboard.BackgroundJobs.ViewModels;

public class JobStatistics
{
    public long EnqueuedCount { get; set; }
    public long ProcessingCount { get; set; }
    public long SucceededCount { get; set; }
    public long FailedCount { get; set; }
    public long ScheduledCount { get; set; }
    public int ServersCount { get; set; }
}