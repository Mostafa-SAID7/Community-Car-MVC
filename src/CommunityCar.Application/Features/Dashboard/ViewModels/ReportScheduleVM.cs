namespace CommunityCar.Application.Features.Dashboard.ViewModels;

public class ReportScheduleVM
{
    public string Name { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public string Schedule { get; set; } = string.Empty; // "daily", "weekly", "monthly"
    public string Format { get; set; } = "PDF";
    public List<string> Recipients { get; set; } = new();
    public bool IsActive { get; set; } = true;
}