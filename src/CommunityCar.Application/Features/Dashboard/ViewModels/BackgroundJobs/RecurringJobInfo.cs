namespace CommunityCar.Application.Features.Dashboard.ViewModels.BackgroundJobs;

public class RecurringJobInfo
{
    public string Id { get; set; } = string.Empty;
    public string Cron { get; set; } = string.Empty;
    public DateTime? NextExecution { get; set; }
    public DateTime? LastExecution { get; set; }
    public string? LastJobState { get; set; }
    public string? Error { get; set; }
}