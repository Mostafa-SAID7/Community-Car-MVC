using CommunityCar.Domain.Enums.Shared;
using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Features.Community.Feed.ViewModels;

public class FeedRequestVM
{
    public Guid? UserId { get; set; }
    public string? FeedType { get; set; } // Personal, Following, Trending, etc.
    public List<string>? ContentTypes { get; set; } // Post, Story, Event, etc.
    public List<string>? Categories { get; set; }
    public List<string>? Tags { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public FeedSortBy SortBy { get; set; } = FeedSortBy.Newest;
    public SortOrder SortOrder { get; set; } = SortOrder.Descending;
    public bool IncludeInteractions { get; set; } = true;
}