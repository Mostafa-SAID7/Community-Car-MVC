namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Overview.Media.SoftDelete.ViewModels;

/// <summary>
/// Summary of recently deleted content
/// </summary>
public class DeletedContentSummaryVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty; // Post, Story, Group, etc.
    public string AuthorName { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public DateTime DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    public string? DeletedReason { get; set; }
    public bool CanRestore { get; set; }
    public bool CanPermanentDelete { get; set; }
}





