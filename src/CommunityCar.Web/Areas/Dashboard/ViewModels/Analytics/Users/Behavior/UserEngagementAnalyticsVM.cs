namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Analytics.Users.Behavior;

/// <summary>
/// User engagement analytics view model
/// </summary>
public class UserEngagementAnalyticsVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public double EngagementScore { get; set; }
    public int TotalInteractions { get; set; }
    public int PostsCreated { get; set; }
    public int CommentsCreated { get; set; }
    public int LikesGiven { get; set; }
    public int SharesCount { get; set; }
    public int ProfileViews { get; set; }
    public double AverageTimePerSession { get; set; }
    public int DaysActive { get; set; }
    public string EngagementLevel { get; set; } = string.Empty; // Low, Medium, High
    public Dictionary<string, int> InteractionsByType { get; set; } = new();
}




