namespace CommunityCar.Application.Features.Community.Feed.ViewModels;


public class FeedFilterVM
{
    public List<string> ContentTypes { get; set; } = new();
    public List<string> Categories { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public List<string> Authors { get; set; } = new();
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int? MinLikes { get; set; }
    public int? MinComments { get; set; }
    public bool? HasImages { get; set; }
    public bool? HasVideos { get; set; }
    public bool? IsVerified { get; set; }
}