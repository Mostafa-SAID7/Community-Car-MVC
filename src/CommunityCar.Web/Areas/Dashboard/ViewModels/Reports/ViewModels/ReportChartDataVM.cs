using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.ViewModels;

public class ReportChartDataVM
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public List<ChartDataVM> Data { get; set; } = new();
    public Dictionary<string, object> Options { get; set; } = new();
}




