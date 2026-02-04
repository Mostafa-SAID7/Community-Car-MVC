namespace CommunityCar.Application.Features.Dashboard.SoftDelete.ViewModels;

/// <summary>
/// Filter model for deleted content search
/// </summary>
public class DeletedContentFilterVM
{
    public string? SearchTerm { get; set; }
    public string? ContentType { get; set; } // Post, Story, Group, Comment
    public Guid? AuthorId { get; set; }
    public DateTime? DeletedAfter { get; set; }
    public DateTime? DeletedBefore { get; set; }
    public string? DeletedBy { get; set; }
    public string? SortBy { get; set; } = "DeletedAt";
    public bool SortDescending { get; set; } = true;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
