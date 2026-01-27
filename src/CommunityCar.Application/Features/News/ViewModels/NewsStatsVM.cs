namespace CommunityCar.Application.Features.News.ViewModels;

public class NewsStatsVM
{
    public int TotalNews { get; set; }
    public int PublishedNews { get; set; }
    public int DraftNews { get; set; }
    public int FeaturedNews { get; set; }
    public int PinnedNews { get; set; }
    public int TotalViews { get; set; }
    public int TotalLikes { get; set; }
    public int TotalComments { get; set; }
    public int TotalShares { get; set; }
    public int NewsThisMonth { get; set; }
    public int NewsThisWeek { get; set; }
    public int NewsToday { get; set; }
    public double AverageViewsPerNews { get; set; }
    public double AverageLikesPerNews { get; set; }
    public double AverageCommentsPerNews { get; set; }
}


