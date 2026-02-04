using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Features.Dashboard.Analytics.ViewModels;

public class ContentAnalyticsVM
{
    public int TotalPosts { get; set; }
    public int TotalComments { get; set; }
    public int TotalLikes { get; set; }
    public int TotalShares { get; set; }
    public int TotalViews { get; set; }
    public double EngagementRate { get; set; }
    public double ContentGrowthRate { get; set; }
    public List<ChartDataVM> ContentCreationChart { get; set; } = new();
    public List<ChartDataVM> EngagementChart { get; set; } = new();
    public List<ChartDataVM> ContentTypeChart { get; set; } = new();
    public List<TopContentVM> TopPerformingContent { get; set; } = new();
}