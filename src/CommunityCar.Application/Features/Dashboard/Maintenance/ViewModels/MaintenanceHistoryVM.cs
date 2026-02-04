namespace CommunityCar.Application.Features.Dashboard.Maintenance.ViewModels;

public class MaintenanceHistoryVM
{
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string MaintenanceType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
    public List<string> AffectedServices { get; set; } = new();
}