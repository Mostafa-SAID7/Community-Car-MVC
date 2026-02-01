namespace CommunityCar.Application.Features.Community.Friends.ViewModels;

public class FriendSuggestionVM
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Bio { get; set; }
    public int MutualFriendsCount { get; set; }
    public IEnumerable<string> MutualFriendsNames { get; set; } = new List<string>();
    public string SuggestionReason { get; set; } = string.Empty;
}