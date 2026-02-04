namespace CommunityCar.Application.Features.Dashboard.SoftDelete.ViewModels;

/// <summary>
/// Deleted content search results
/// </summary>
public class DeletedContentSearchVM
{
    public IEnumerable<DeletedContentItemVM> Items { get; set; } = new List<DeletedContentItemVM>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
    public DeletedContentFilterVM Filter { get; set; } = new();
}
