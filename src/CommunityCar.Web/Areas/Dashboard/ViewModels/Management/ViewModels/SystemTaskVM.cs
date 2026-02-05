namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.ViewModels;

public class SystemTaskVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal Progress { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? EndTime { get; set; }
    public TimeSpan? Duration { get; set; }
    public DateTime? NextRun { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public decimal ProgressPercentage { get; set; }
    public string Result { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public int Priority { get; set; }
    public TimeSpan? EstimatedDuration { get; set; }
    public TimeSpan? ActualDuration { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
}




