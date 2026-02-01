namespace CommunityCar.Application.Features.Analytics.ViewModels;

public class UserActivityStatsVM
{
    public int TotalActivities { get; set; }
    public int ViewsCount { get; set; }
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    public int SharesCount { get; set; }
    public int SearchesCount { get; set; }
    public double AverageSessionDuration { get; set; }
    public int ActiveDays { get; set; }
    public DateTime? LastActivity { get; set; }
    public string MostActiveDay { get; set; } = string.Empty;
    public string MostActiveHour { get; set; } = string.Empty;
    public Dictionary<string, int> ActivityBreakdown { get; set; } = new();
    public Dictionary<string, int> DailyActivity { get; set; } = new();
}