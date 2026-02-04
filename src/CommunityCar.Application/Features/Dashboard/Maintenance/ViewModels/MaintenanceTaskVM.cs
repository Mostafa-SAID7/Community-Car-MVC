namespace CommunityCar.Application.Features.Dashboard.Maintenance.ViewModels;

public class MaintenanceTaskVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Pending, InProgress, Completed, Failed
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public TimeSpan? Duration { get; set; }
    public string? AssignedTo { get; set; }
    public int Priority { get; set; } = 1; // 1-5, 5 being highest
    public List<string> Dependencies { get; set; } = new();
    public string? Notes { get; set; }
    public string? ErrorMessage { get; set; }
    public int ProgressPercentage { get; set; } = 0;
}