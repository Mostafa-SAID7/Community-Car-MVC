namespace CommunityCar.Application.Features.Community.Feed.ViewModels;

public class FriendSuggestionVM
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public string? Avatar { get; set; }
    public string Slug { get; set; } = string.Empty;
    public int MutualFriendsCount { get; set; }
    public string? ReasonForSuggestion { get; set; }
    public string? SuggestionReason { get; set; }
    public bool IsFollowing { get; set; }
}