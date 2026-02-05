namespace CommunityCar.Web.Areas.Identity.ViewModels.Social;

public class SuggestedUserVM : FollowingVM
{
    public int MutualFollowersCount { get; set; }
    public List<string> MutualFollowerNames { get; set; } = new();
    public string SuggestionReason { get; set; } = string.Empty;
}
