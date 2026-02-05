using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.ViewModels;

public class ReportDataVM
{
    public Dictionary<string, object> Summary { get; set; } = new();
    public List<Dictionary<string, object>> Details { get; set; } = new();
    public List<ChartDataVM> Charts { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}




