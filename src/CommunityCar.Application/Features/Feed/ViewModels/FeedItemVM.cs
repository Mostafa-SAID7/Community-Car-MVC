using CommunityCar.Domain.Enums;
using CommunityCar.Application.Features.Interactions.ViewModels;

namespace CommunityCar.Application.Features.Feed.ViewModels;

public class FeedItemVM
{
    public Guid Id { get; set; }
    public string ContentType { get; set; } = string.Empty; // Story, News, Review, QA, Post
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? TitleAr { get; set; }
    public string? ContentAr { get; set; }
    public string? SummaryAr { get; set; }
    public string? ImageUrl { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    
    // Author information
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorAvatar { get; set; }
    public bool IsVerified { get; set; }
    
    // Engagement metrics
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public int ShareCount { get; set; }
    public bool IsLikedByUser { get; set; }
    public bool IsBookmarkedByUser { get; set; }
    
    // Content metadata
    public List<string> Tags { get; set; } = new();
    public string? Category { get; set; }
    public string? CarMake { get; set; }
    public string? CarModel { get; set; }
    public int? CarYear { get; set; }
    public string CarDisplayName { get; set; } = string.Empty;
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
    
    // Feed-specific
    public double RelevanceScore { get; set; }
    public string ReasonForShowing { get; set; } = string.Empty; // "Trending", "Friend posted", "Similar interests"
    public bool IsSeen { get; set; }
    public bool IsTrending { get; set; }
    public bool IsFeatured { get; set; }
    
    // Content-specific properties
    public int? Rating { get; set; } // For reviews
    public bool? IsAnswered { get; set; } // For QA
    public bool? IsExpired { get; set; } // For stories
    public string? Location { get; set; } // For stories/events
    
    // Comments
    public List<CommentVM> InitialComments { get; set; } = new();
}


