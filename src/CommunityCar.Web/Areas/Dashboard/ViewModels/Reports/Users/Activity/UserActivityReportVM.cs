namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.Users.Activity;

/// <summary>
/// User activity report view model
/// </summary>
public class UserActivityReportVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime ReportDate { get; set; }
    public int LoginCount { get; set; }
    public int PostsCreated { get; set; }
    public int CommentsCreated { get; set; }
    public int LikesGiven { get; set; }
    public int SharesCount { get; set; }
    public int ProfileViews { get; set; }
    public TimeSpan TotalTimeSpent { get; set; }
    public int SessionCount { get; set; }
    public double AverageSessionDuration { get; set; }
    public string MostActiveSection { get; set; } = string.Empty;
    public List<string> DevicesUsed { get; set; } = new();
    public Dictionary<string, int> HourlyActivity { get; set; } = new();
}




