namespace CommunityCar.Application.Features.Dashboard.Reports.ViewModels;

public class ReportGenerationVM
{
    public string ReportType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<string> SelectedMetrics { get; set; } = new();
    public List<string> SelectedFilters { get; set; } = new();
    public string Format { get; set; } = "PDF"; // PDF, Excel, CSV
    public bool IncludeCharts { get; set; } = true;
    public bool IncludeRawData { get; set; } = false;
    public string EmailTo { get; set; } = string.Empty;
    public bool ScheduleReport { get; set; } = false;
    public string ScheduleFrequency { get; set; } = string.Empty; // Daily, Weekly, Monthly
}