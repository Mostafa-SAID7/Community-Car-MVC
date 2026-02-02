namespace CommunityCar.AI.Models;

/// <summary>
/// User behavior prediction data
/// </summary>
public class UserBehaviorData
{
    public string UserId { get; set; } = string.Empty;
    public int PostsCount { get; set; }
    public int CommentsCount { get; set; }
    public int LikesGiven { get; set; }
    public int LikesReceived { get; set; }
    public int SharesCount { get; set; }
    public float EngagementRate { get; set; }
    public int DaysActive { get; set; }
    public float AverageSentiment { get; set; }
    public int ReportsReceived { get; set; }
    public bool IsActive { get; set; }
    public Dictionary<string, float> ActivityMetrics { get; set; } = new();
    public List<string> Interests { get; set; } = new();
    public Dictionary<string, int> InteractionCounts { get; set; } = new();
    public DateTime LastActivity { get; set; }
    public string UserSegment { get; set; } = string.Empty;
}