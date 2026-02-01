using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Features.Community.News.ViewModels;

public class NewsVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public NewsCategory Category { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
    public bool IsPublished { get; set; }
    public List<string> Tags { get; set; } = new();
    public string? ImageUrl { get; set; }
    public DateTime PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int ViewCount { get; set; }
    public int CommentCount { get; set; }
    public int ShareCount { get; set; }
    
    public string TimeAgo
    {
        get
        {
            var timeAgo = DateTime.UtcNow - PublishedAt;
            return timeAgo.TotalDays > 7 ? PublishedAt.ToString("MMM dd, yyyy") :
                   timeAgo.TotalDays >= 1 ? $"{(int)timeAgo.TotalDays} days ago" :
                   timeAgo.TotalHours >= 1 ? $"{(int)timeAgo.TotalHours} hours ago" :
                   "Just now";
        }
    }
}