using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Features.Community.Posts.ViewModels;

public class PostsSearchVM
{
    // Search parameters
    public string? SearchTerm { get; set; }
    public string? Query { get; set; }
    public PostType? Type { get; set; }
    public string? Category { get; set; }
    public Guid? GroupId { get; set; }
    public Guid? AuthorId { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool? IsPinned { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortBy { get; set; } = "CreatedAt";
    public string SortOrder { get; set; } = "desc";
    
    // Results
    public List<PostSummaryVM> Items { get; set; } = new();
    public List<PostVM> Results { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}