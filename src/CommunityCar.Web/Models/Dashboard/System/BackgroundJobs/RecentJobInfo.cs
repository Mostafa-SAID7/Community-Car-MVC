namespace CommunityCar.Web.Models.Dashboard.System.BackgroundJobs;

public class RecentJobInfo
{
    public string Id { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; }
    public TimeSpan? Duration { get; set; }
    public string? Error { get; set; }
}