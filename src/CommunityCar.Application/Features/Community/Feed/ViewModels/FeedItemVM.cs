using System.ComponentModel.DataAnnotations;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;
using CommunityCar.Application.Features.Shared.Interactions.ViewModels;


namespace CommunityCar.Application.Features.Community.Feed.ViewModels;

public class FeedItemVM
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty; // Post, Story, Event, etc.
    public string ContentType { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorAvatar { get; set; } = string.Empty;
    public string AuthorSlug { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? TitleAr { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ContentAr { get; set; }
    public string? Summary { get; set; }
    public string? SummaryAr { get; set; }
    public string? ImageUrl { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int LikesCount { get; set; }
    public int LikeCount { get; set; }
    public int CommentsCount { get; set; }
    public int CommentCount { get; set; }
    public int SharesCount { get; set; }
    public int ShareCount { get; set; }
    public int ViewsCount { get; set; }
    public int ViewCount { get; set; }
    public bool IsLiked { get; set; }
    public bool IsLikedByUser { get; set; }
    public bool IsBookmarked { get; set; }
    public bool IsBookmarkedByUser { get; set; }
    public bool IsFollowing { get; set; }
    public bool IsTrending { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsAnswered { get; set; }
    public bool IsExpired { get; set; }
    public double RelevanceScore { get; set; }
    public string? ReasonForShowing { get; set; }
    public string Url { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public string Category { get; set; } = string.Empty;
    public ContentStatus Status { get; set; }
    public Priority Priority { get; set; }
    public string? CarMake { get; set; }
    public string? CarModel { get; set; }
    public int? CarYear { get; set; }
    public string? CarDisplayName { get; set; }
    public string? Location { get; set; }
    public int? Rating { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
    public List<CommentVM> InitialComments { get; set; } = new();
}