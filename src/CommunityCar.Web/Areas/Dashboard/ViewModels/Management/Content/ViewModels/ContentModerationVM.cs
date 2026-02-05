using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.Content.ViewModels;

public class ContentModerationVM
{
    public List<ModerationItemVM> PendingItems { get; set; } = new();
    public List<ModerationItemVM> RecentlyModerated { get; set; } = new();
    public int TotalPendingItems { get; set; }
    public int TotalModeratedToday { get; set; }
    public int TotalApprovedToday { get; set; }
    public int TotalRejectedToday { get; set; }
    public int TotalDeletedToday { get; set; }
    public int TotalReports { get; set; }
    public int AutoFlaggedItems { get; set; }
    public double AverageResponseTime { get; set; }
    public List<ChartDataVM> ModerationStatsChart { get; set; } = new();
}




