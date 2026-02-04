namespace CommunityCar.Application.Features.Community.News.ViewModels;

public class NewsVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string? FeaturedImageUrl { get; set; }
    public List<string> Tags { get; set; } = new();
    public string Category { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public bool IsFeatured { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? PublishDate { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorAvatar { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public int ShareCount { get; set; }
    public bool AllowComments { get; set; }
    public string Slug { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}