using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.ViewModels;

public class ReportSectionVM
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public object Data { get; set; } = new();
    public List<ChartDataVM> Charts { get; set; } = new();
    public int Order { get; set; }
}




