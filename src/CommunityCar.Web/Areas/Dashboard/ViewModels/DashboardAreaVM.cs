using CommunityCar.Web.Areas.Dashboard.ViewModels.Overview.ViewModels;

namespace CommunityCar.Web.Areas.Dashboard.ViewModels;

public class DashboardAreaVM
{
    public string PageTitle { get; set; } = "Dashboard Overview";
    public OverviewVM Overview { get; set; } = new();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}





