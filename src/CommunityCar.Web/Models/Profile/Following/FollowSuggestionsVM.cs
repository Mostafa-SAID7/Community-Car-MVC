namespace CommunityCar.Web.Models.Profile.Following;

/// <summary>
/// View model for follow suggestions
/// </summary>
public class FollowSuggestionsVM
{
    public List<SuggestedUserVM> SuggestedUsers { get; set; } = new();
    public string SuggestionReason { get; set; } = string.Empty;
}