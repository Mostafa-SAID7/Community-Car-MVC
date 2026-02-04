using System.ComponentModel.DataAnnotations;
using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Features.Dashboard.Maintenance.ViewModels;

public class MaintenanceScheduleVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime ScheduledStartTime { get; set; }
    public DateTime ScheduledEndTime { get; set; }
    public TimeSpan EstimatedDuration { get; set; }
    public string Status { get; set; } = string.Empty; // Scheduled, InProgress, Completed, Cancelled
    public string ScheduledBy { get; set; } = string.Empty;
    public List<string> AffectedServices { get; set; } = new();
    public List<string> NotificationRecipients { get; set; } = new();
    public bool NotifyUsers { get; set; } = true;
    public string? NotificationMessage { get; set; }
    public List<MaintenanceTaskVM> Tasks { get; set; } = new();
}