namespace CommunityCar.Application.Features.Dashboard.Management.Media.SoftDelete.ViewModels;

/// <summary>
/// Deleted content item view model
/// </summary>
public class DeletedContentItemVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string ContentPreview { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    public string? DeletedReason { get; set; }
    public bool CanRestore { get; set; }
    public bool CanPermanentDelete { get; set; }
    public string Slug { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}
