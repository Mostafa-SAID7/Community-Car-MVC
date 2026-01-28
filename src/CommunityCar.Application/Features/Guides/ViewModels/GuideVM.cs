using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Features.Guides.ViewModels;

public class GuideVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; }
    
    // Arabic Localization
    public string? TitleAr { get; set; }
    public string? ContentAr { get; set; }
    public string? SummaryAr { get; set; }
    
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorAvatar { get; set; }
    
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
    public bool IsVerified { get; set; }
    public bool IsFeatured { get; set; }
    
    public string? Category { get; set; }
    public GuideDifficulty Difficulty { get; set; }
    public string DifficultyText => Difficulty.ToString();
    public int EstimatedMinutes { get; set; }
    public string EstimatedTimeText => EstimatedMinutes < 60 
        ? $"{EstimatedMinutes} min" 
        : $"{EstimatedMinutes / 60}h {EstimatedMinutes % 60}m";
    
    public string? ThumbnailUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    
    public int ViewCount { get; set; }
    public int BookmarkCount { get; set; }
    public double AverageRating { get; set; }
    public int RatingCount { get; set; }
    
    public List<string> Tags { get; set; } = new();
    public List<string> Prerequisites { get; set; } = new();
    public List<string> RequiredTools { get; set; } = new();
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public string TimeAgo { get; set; } = string.Empty;
    public bool IsBookmarked { get; set; }
    public double? UserRating { get; set; }
}


