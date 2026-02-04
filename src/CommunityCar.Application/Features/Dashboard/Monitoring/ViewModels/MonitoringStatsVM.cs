namespace CommunityCar.Application.Features.Dashboard.Monitoring.ViewModels;

/// <summary>
/// ViewModel for monitoring statistics
/// </summary>
public class MonitoringStatsVM
{
    public int TotalServices { get; set; }
    public int HealthyServices { get; set; }
    public int UnhealthyServices { get; set; }
    public int CriticalAlerts { get; set; }
    public int WarningAlerts { get; set; }
    public int InfoAlerts { get; set; }
    public int UnreadAlerts { get; set; }
    public int PendingModerationItems { get; set; }
    public double SystemUptime { get; set; }
    public double AverageResponseTime { get; set; }
    public DateTime LastHealthCheck { get; set; }
    public Dictionary<string, string> ServiceStatuses { get; set; } = new();
    public List<string> RecentAlerts { get; set; } = new();
}