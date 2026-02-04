namespace CommunityCar.Application.Features.Dashboard.Reports.ViewModels;

public class ReportChartVM
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Bar, Line, Pie, etc.
    public string DataSource { get; set; } = string.Empty;
    public Dictionary<string, object> Configuration { get; set; } = new();
}