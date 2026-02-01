using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Features.Community.Posts.ViewModels;

public class UpdatePostVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? TitleAr { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ContentAr { get; set; }
    public PostType Type { get; set; }
    public string? Category { get; set; }
    public bool AllowComments { get; set; }
    public bool IsPinned { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<string> ImageUrls { get; set; } = new();
}