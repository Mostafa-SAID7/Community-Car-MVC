using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Features.News.ViewModels;

public class NewsItemVM
{
    public Guid Id { get; set; }
    public string Headline { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? ImageUrl { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public bool IsPublished { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsPinned { get; set; }
    
    // Engagement metrics
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public int ShareCount { get; set; }
    
    // Content categorization
    public NewsCategory Category { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? Source { get; set; }
    public string? SourceUrl { get; set; }
    
    // SEO and metadata
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? Slug { get; set; }
    
    // Tags and topics
    public IEnumerable<string> Tags { get; set; } = new List<string>();
    
    // Additional media
    public IEnumerable<string> ImageUrls { get; set; } = new List<string>();
    
    // Automotive specific
    public string? CarMake { get; set; }
    public string? CarModel { get; set; }
    public int? CarYear { get; set; }
    public string CarDisplayName { get; set; } = string.Empty;
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Calculated properties
    public string TimeAgo { get; set; } = string.Empty;
    public string ReadingTime { get; set; } = string.Empty;
    public bool HasMultipleImages { get; set; }
    public string Excerpt { get; set; } = string.Empty;
}