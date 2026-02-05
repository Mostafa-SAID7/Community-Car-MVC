namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Analytics.Users.Behavior;

public class AnalyticsSuggestionVM
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public string SuggestionReason { get; set; } = string.Empty;
    public double RelevanceScore { get; set; }
    public int MutualFriendsCount { get; set; }
    public IEnumerable<string> CommonInterests { get; set; } = new List<string>();
    public bool IsVerified { get; set; }
    public string? Location { get; set; }
    public int FollowersCount { get; set; }
    public int PostsCount { get; set; }
}




