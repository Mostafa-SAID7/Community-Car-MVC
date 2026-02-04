namespace CommunityCar.Application.Features.Dashboard.Maintenance.ViewModels;

public class MaintenanceStatusVM
{
    public bool IsMaintenanceMode { get; set; }
    public string MaintenanceMessage { get; set; } = string.Empty;
    public DateTime? ScheduledStart { get; set; }
    public DateTime? ScheduledEnd { get; set; }
    public DateTime? ActualStart { get; set; }
    public DateTime? ActualEnd { get; set; }
    public string Status { get; set; } = string.Empty;
    public string MaintenanceType { get; set; } = string.Empty;
    public List<string> AffectedServices { get; set; } = new();
    public string CreatedBy { get; set; } = string.Empty;
    public List<MaintenanceTaskVM> Tasks { get; set; } = new();
}

public class MaintenanceHistoryVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string MaintenanceType { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
    public TimeSpan? Duration { get; set; }
    public bool WasSuccessful { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class MaintenanceTaskVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string AssignedTo { get; set; } = string.Empty;
    public int Priority { get; set; }
    public decimal ProgressPercentage { get; set; }
}