using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Features.Community.News.ViewModels;

public class NewsStatsVM
{
    public int TotalNews { get; set; }
    public int PublishedNews { get; set; }
    public int DraftNews { get; set; }
    public int FeaturedNews { get; set; }
    public int NewsToday { get; set; }
    public int NewsThisWeek { get; set; }
    public int NewsThisMonth { get; set; }
    public int TotalViews { get; set; }
    public int TotalLikes { get; set; }
    public int TotalComments { get; set; }
    public int TotalShares { get; set; }
    public double AverageEngagement { get; set; }
    public List<ChartDataVM> PublishingTrend { get; set; } = new();
    public Dictionary<string, int> NewsByCategory { get; set; } = new();
    public List<string> PopularTags { get; set; } = new();
    public List<NewsVM> TopNews { get; set; } = new();
}