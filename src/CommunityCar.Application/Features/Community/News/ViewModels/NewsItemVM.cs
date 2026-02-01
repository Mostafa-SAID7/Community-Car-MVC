using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Features.Community.News.ViewModels;

public class NewsItemVM
{
    public Guid Id { get; set; }
    public string Headline { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? HeadlineAr { get; set; }
    public string? BodyAr { get; set; }
    public string? SummaryAr { get; set; }
    public string? ImageUrl { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public NewsCategory Category { get; set; }
    public string? CarMake { get; set; }
    public string? CarModel { get; set; }
    public int? CarYear { get; set; }
    public List<string> Tags { get; set; } = new();
    public string? Source { get; set; }
    public string? SourceUrl { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorProfilePicture { get; set; }
    public bool IsPublished { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsPinned { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public int ShareCount { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string Slug { get; set; } = string.Empty;
    public bool IsLikedByCurrentUser { get; set; }
    public bool IsBookmarkedByCurrentUser { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    
    // Computed properties
    public string CategoryName => Category.ToString();
    public string TimeAgo { get; set; } = string.Empty;
    public int ReadingTime { get; set; }
    public bool HasMultipleImages => ImageUrls.Count > 1;
    public string Excerpt { get; set; } = string.Empty;
}


