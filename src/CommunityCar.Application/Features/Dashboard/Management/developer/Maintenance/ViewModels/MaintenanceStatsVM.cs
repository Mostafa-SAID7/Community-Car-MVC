namespace CommunityCar.Application.Features.Dashboard.Management.developer.Maintenance.ViewModels;

public class MaintenanceStatsVM
{
    public int TotalMaintenanceEvents { get; set; }
    public int TotalMaintenanceWindows { get; set; }
    public int ScheduledMaintenances { get; set; }
    public int ScheduledMaintenanceCount { get; set; }
    public int EmergencyMaintenances { get; set; }
    public int EmergencyMaintenanceCount { get; set; }
    public double AverageDowntimeMinutes { get; set; }
    public double TotalDowntimeHours { get; set; }
    public double SuccessfulMaintenanceRate { get; set; }
    public double AverageNotificationsSent { get; set; }
    public DateTime? LastMaintenance { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public DateTime? NextScheduledMaintenance { get; set; }
    public string MaintenanceFrequency { get; set; } = string.Empty;
    public string PreferredMaintenanceWindow { get; set; } = string.Empty;
}