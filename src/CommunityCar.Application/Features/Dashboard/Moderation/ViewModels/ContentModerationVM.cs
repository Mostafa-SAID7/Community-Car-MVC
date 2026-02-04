using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Features.Dashboard.Moderation.ViewModels;

public class ContentModerationVM
{
    public List<ModerationItemVM> PendingItems { get; set; } = new();
    public List<ModerationItemVM> RecentlyModerated { get; set; } = new();
    public int TotalPendingItems { get; set; }
    public int TotalModeratedToday { get; set; }
    public int TotalApprovedToday { get; set; }
    public int TotalRejectedToday { get; set; }
    public List<ChartDataVM> ModerationStatsChart { get; set; } = new();
}