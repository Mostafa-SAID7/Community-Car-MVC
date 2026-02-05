using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.ViewModels;

public class ReportGenerationVM
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<string> IncludedSections { get; set; } = new();
    public string Format { get; set; } = string.Empty;
    public bool IncludeCharts { get; set; }
    public bool IncludeRawData { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
}




