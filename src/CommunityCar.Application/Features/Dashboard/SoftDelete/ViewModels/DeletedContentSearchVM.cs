namespace CommunityCar.Application.Features.Dashboard.SoftDelete.ViewModels;

/// <summary>
/// ViewModel for searching and managing soft-deleted content
/// </summary>
public class DeletedContentSearchVM
{
    public string? SearchTerm { get; set; }
    public string? ContentType { get; set; } // Post, Comment, Story, etc.
    public DateTime? DeletedAfter { get; set; }
    public DateTime? DeletedBefore { get; set; }
    public string? DeletedBy { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortBy { get; set; } = "DeletedAt";
    public string SortDirection { get; set; } = "desc";
    
    // Results
    public List<DeletedContentItemVM> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}