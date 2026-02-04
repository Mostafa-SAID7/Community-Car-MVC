namespace CommunityCar.Application.Features.Dashboard.Reports.ViewModels;

public class ReportDataVM
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public Dictionary<string, object> Summary { get; set; } = new();
    public List<Dictionary<string, object>> Data { get; set; } = new();
    public List<ReportChartDataVM> Charts { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}