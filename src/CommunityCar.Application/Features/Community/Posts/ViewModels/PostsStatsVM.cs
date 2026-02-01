using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Features.Community.Posts.ViewModels;

public class PostsStatsVM
{
    public int TotalPosts { get; set; }
    public int TextPosts { get; set; }
    public int ImagePosts { get; set; }
    public int VideoPosts { get; set; }
    public int LinkPosts { get; set; }
    public int PollPosts { get; set; }
    public List<PostSummaryVM> RecentPosts { get; set; } = new();
    public Dictionary<string, int> PostsByType { get; set; } = new();
}