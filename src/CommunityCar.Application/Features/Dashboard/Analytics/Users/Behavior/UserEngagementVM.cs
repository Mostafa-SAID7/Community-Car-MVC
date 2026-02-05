namespace CommunityCar.Application.Features.Dashboard.Analytics.Users.Behavior;

/// <summary>
/// User engagement view model for behavior analytics
/// </summary>
public class UserEngagementVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public double EngagementScore { get; set; }
    public int TotalInteractions { get; set; }
    public int LikesGiven { get; set; }
    public int CommentsGiven { get; set; }
    public int SharesGiven { get; set; }
    public int ViewsGiven { get; set; }
    public DateTime LastEngagement { get; set; }
    public Dictionary<string, int> EngagementByType { get; set; } = new();
}