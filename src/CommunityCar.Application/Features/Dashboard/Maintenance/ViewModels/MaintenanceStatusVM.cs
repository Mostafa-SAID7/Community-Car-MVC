namespace CommunityCar.Application.Features.Dashboard.Maintenance.ViewModels;

public class MaintenanceStatusVM
{
    public bool IsMaintenanceModeEnabled { get; set; }
    public string? MaintenanceMessage { get; set; }
    public DateTime? ScheduledStart { get; set; }
    public DateTime? ScheduledEnd { get; set; }
    public string? MaintenanceType { get; set; }
    public List<string> AffectedServices { get; set; } = new();
}