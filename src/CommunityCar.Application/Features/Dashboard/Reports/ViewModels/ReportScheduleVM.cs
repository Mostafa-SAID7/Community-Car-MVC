namespace CommunityCar.Application.Features.Dashboard.Reports.ViewModels;

public class ReportScheduleVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty; // Daily, Weekly, Monthly
    public string Recipients { get; set; } = string.Empty;
    public DateTime NextRunDate { get; set; }
    public DateTime LastRunDate { get; set; }
    public bool IsActive { get; set; } = true;
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}