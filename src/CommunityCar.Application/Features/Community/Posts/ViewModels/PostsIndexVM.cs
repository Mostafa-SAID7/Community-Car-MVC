using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Features.Community.Posts.ViewModels;

public class PostsIndexVM
{
    public PostsSearchVM SearchRequest { get; set; } = new();
    public PostsSearchVM SearchResponse { get; set; } = new();
    public PostsStatsVM Stats { get; set; } = new();
}