using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Features.Dashboard.Reports.ViewModels;

public class ReportScheduleVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty; // daily, weekly, monthly
    public string CronExpression { get; set; } = string.Empty;
    public DateTime NextRun { get; set; }
    public DateTime? LastRun { get; set; }
    public bool IsActive { get; set; }
    public List<string> Recipients { get; set; } = new();
    public string Format { get; set; } = string.Empty;
    public ReportGenerationVM Template { get; set; } = new();
}