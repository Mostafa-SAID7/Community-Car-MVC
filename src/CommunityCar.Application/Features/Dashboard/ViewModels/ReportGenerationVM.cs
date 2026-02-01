namespace CommunityCar.Application.Features.Dashboard.ViewModels;

public class ReportGenerationVM
{
    public string ReportType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Format { get; set; } = "PDF";
    public List<string> Metrics { get; set; } = new();
    public Dictionary<string, object> Parameters { get; set; } = new();
}