using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Shared.ViewModels;

public class CommentSearchVM
{
    public string? Query { get; set; }
    public EntityType? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public Guid? AuthorId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortBy { get; set; } = "CreatedAt";
    public string SortOrder { get; set; } = "desc";
    
    // Additional search properties
    public Guid? UserId { get; set; }
    public bool? TopLevelOnly { get; set; }
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }
    
    // Results
    public List<CommentVM> Results { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}