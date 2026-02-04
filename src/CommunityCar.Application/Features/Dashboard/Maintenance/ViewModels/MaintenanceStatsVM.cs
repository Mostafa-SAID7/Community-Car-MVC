namespace CommunityCar.Application.Features.Dashboard.Maintenance.ViewModels;

public class MaintenanceStatsVM
{
    public int TotalMaintenanceEvents { get; set; }
    public int ScheduledMaintenances { get; set; }
    public int EmergencyMaintenances { get; set; }
    public double AverageDowntimeMinutes { get; set; }
    public DateTime? LastMaintenance { get; set; }
    public DateTime? NextScheduledMaintenance { get; set; }
}