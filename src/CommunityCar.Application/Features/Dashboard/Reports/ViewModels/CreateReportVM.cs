namespace CommunityCar.Application.Features.Dashboard.Reports.ViewModels;

public class CreateReportVM
{
    public string ReportType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; } = DateTime.Now.AddDays(-30);
    public DateTime EndDate { get; set; } = DateTime.Now;
    public List<string> SelectedCategories { get; set; } = new();
    public List<string> SelectedMetrics { get; set; } = new();
    public string OutputFormat { get; set; } = "PDF";
    public bool IncludeCharts { get; set; } = true;
    public bool IncludeDetails { get; set; } = true;
    public bool SendByEmail { get; set; } = false;
    public string EmailRecipients { get; set; } = string.Empty;
}