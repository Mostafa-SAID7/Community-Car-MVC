using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Features.Community.Posts.ViewModels;

public class CreatePostVM
{
    public string Title { get; set; } = string.Empty;
    public string? TitleAr { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ContentAr { get; set; }
    public PostType Type { get; set; } = PostType.Text;
    public string? Category { get; set; }
    public Guid? GroupId { get; set; }
    public bool AllowComments { get; set; } = true;
    public List<string> Tags { get; set; } = new();
    public List<string> ImageUrls { get; set; } = new();
}