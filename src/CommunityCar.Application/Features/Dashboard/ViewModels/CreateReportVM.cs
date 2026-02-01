namespace CommunityCar.Application.Features.Dashboard.ViewModels;

public class CreateReportVM
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Format { get; set; } = "PDF";
    public bool IsPublic { get; set; }
    public List<string> IncludeMetrics { get; set; } = new();
}