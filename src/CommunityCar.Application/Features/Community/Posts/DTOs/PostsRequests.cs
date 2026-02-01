using System.ComponentModel.DataAnnotations;
using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Features.Community.Posts.DTOs;

public class CreatePostRequest
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [StringLength(10000)]
    public string Content { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string? TitleAr { get; set; }

    public string? ContentAr { get; set; }
    
    public PostType Type { get; set; } = PostType.Text;
    
    public Guid? GroupId { get; set; }
    
    public List<string>? Tags { get; set; }
    
    public List<string>? ImageUrls { get; set; }
}

public class UpdatePostRequest
{
    public Guid Id { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [StringLength(10000)]
    public string Content { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string? TitleAr { get; set; }

    public string? ContentAr { get; set; }
    
    public PostType Type { get; set; } = PostType.Text;
    
    public string? Category { get; set; }
    
    public bool IsPinned { get; set; }
    
    public bool AllowComments { get; set; } = true;
    
    public List<string>? Tags { get; set; }
    
    public List<string>? ImageUrls { get; set; }
}

public class PostsSearchRequest
{
    public string? SearchTerm { get; set; }
    public PostType? Type { get; set; }
    public Guid? AuthorId { get; set; }
    public Guid? GroupId { get; set; }
    public string? SortBy { get; set; } = "newest"; // newest, oldest, title
    
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;
    
    [Range(1, 100)]
    public int PageSize { get; set; } = 20;
}

public class PostsSearchResponse
{
    public IEnumerable<PostSummaryVM> Items { get; set; } = new List<PostSummaryVM>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}

public class PostSummaryVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? TitleAr { get; set; }
    public string? ContentAr { get; set; }
    public PostType Type { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public Guid? GroupId { get; set; }
    public string? GroupName { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<string> ImageUrls { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Helper properties
    public string TypeText => Type.ToString();
    public string ContentPreview => Content.Length > 200 ? Content.Substring(0, 200) + "..." : Content;
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
}


