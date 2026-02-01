namespace CommunityCar.Application.Features.Dashboard.ViewModels;

public class UserAnalyticsVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int LoginCount { get; set; }
    public int PostsCreated { get; set; }
    public int QuestionsAsked { get; set; }
    public int AnswersGiven { get; set; }
    public int ReviewsWritten { get; set; }
    public int StoriesShared { get; set; }
    public int InteractionsCount { get; set; }
    public TimeSpan TimeSpentOnSite { get; set; }
    public int PageViews { get; set; }
    public string MostVisitedSection { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public string BrowserType { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;

    public int NewUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int ReturnUsers { get; set; }
    public decimal RetentionRate { get; set; }
    public decimal ChurnRate { get; set; }
    public TimeSpan AverageSessionDuration { get; set; }
    public List<ChartDataVM> UserGrowthData { get; set; } = new();
    public List<ChartDataVM> ActivityData { get; set; } = new();
}