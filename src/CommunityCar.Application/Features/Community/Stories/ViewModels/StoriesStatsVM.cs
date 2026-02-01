namespace CommunityCar.Application.Features.Community.Stories.ViewModels;

public class StoriesStatsVM
{
    public int TotalStories { get; set; }
    public int ActiveStories { get; set; }
    public int ExpiredStories { get; set; }
    public int ArchivedStories { get; set; }
    public int FeaturedStories { get; set; }
    public int HighlightedStories { get; set; }
    public int TotalViews { get; set; }
    public int TotalLikes { get; set; }
    public int TotalComments { get; set; }
    public int TotalShares { get; set; }
    public int TotalReplies { get; set; }
    public int StoriesThisMonth { get; set; }
    public int StoriesThisWeek { get; set; }
    public int StoriesToday { get; set; }
    public double AverageViewsPerStory { get; set; }
    public double AverageLikesPerStory { get; set; }
    public double AverageLifespanHours { get; set; }
    public double EngagementRate { get; set; }
    public List<string> PopularTags { get; set; } = new();
    public List<string> TrendingLocations { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}