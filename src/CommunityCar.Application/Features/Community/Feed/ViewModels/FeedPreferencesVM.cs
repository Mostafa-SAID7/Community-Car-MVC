using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Community.Feed.ViewModels;


public class FeedPreferencesVM
{
    public Guid UserId { get; set; }
    public List<string> PreferredContentTypes { get; set; } = new();
    public List<string> PreferredCategories { get; set; } = new();
    public List<string> PreferredTags { get; set; } = new();
    public List<Guid> FollowingUsers { get; set; } = new();
    public List<Guid> BlockedUsers { get; set; } = new();
    public List<string> MutedKeywords { get; set; } = new();
    public bool ShowNSFW { get; set; } = false;
    public bool ShowSponsoredContent { get; set; } = true;
    public string DefaultSortBy { get; set; } = "CreatedAt";
    public SortOrder DefaultSortOrder { get; set; } = SortOrder.Descending;
    public int DefaultPageSize { get; set; } = 20;
}