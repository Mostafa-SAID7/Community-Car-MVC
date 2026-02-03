using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Features.Community.Posts.ViewModels;

public class PostVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? TitleAr { get; set; }
    public string? ContentAr { get; set; }
    public PostType Type { get; set; }
    public string? Category { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorProfilePicture { get; set; }
    public Guid? GroupId { get; set; }
    public string? GroupName { get; set; }
    public bool IsPinned { get; set; }
    public bool AllowComments { get; set; } = true;
    public List<string> Tags { get; set; } = new();
    public List<string> ImageUrls { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Interaction properties
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public int ShareCount { get; set; }
    public int BookmarkCount { get; set; }
    public bool IsLiked { get; set; }
    public bool IsBookmarked { get; set; }
    
    // Helper properties
    public string TypeText => Type.ToString();
    public string TypeIcon => Type switch
    {
        PostType.Text => "file-text",
        PostType.Image => "image",
        PostType.Video => "video",
        PostType.Link => "link",
        PostType.Poll => "bar-chart-3",
        _ => "file-text"
    };
    
    public string TimeAgo
    {
        get
        {
            var timeAgo = DateTime.UtcNow - CreatedAt;
            return timeAgo.TotalDays > 7 ? CreatedAt.ToString("MMM dd, yyyy") :
                   timeAgo.TotalDays >= 1 ? $"{(int)timeAgo.TotalDays} days ago" :
                   timeAgo.TotalHours >= 1 ? $"{(int)timeAgo.TotalHours} hours ago" :
                   "Just now";
        }
    }
    
    public bool IsEdited => UpdatedAt.HasValue && UpdatedAt > CreatedAt.AddMinutes(5);
}