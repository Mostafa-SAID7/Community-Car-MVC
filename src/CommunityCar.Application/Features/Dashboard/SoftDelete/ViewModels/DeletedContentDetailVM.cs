namespace CommunityCar.Application.Features.Dashboard.SoftDelete.ViewModels;

public class DeletedContentDetailVM
{
    public Guid Id { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime DeletedAt { get; set; }
    public string DeletedBy { get; set; } = string.Empty;
    public string DeleteReason { get; set; } = string.Empty;
    public bool CanRestore { get; set; }
    public DateTime? PermanentDeletionDate { get; set; }
    public List<RelatedContentVM> RelatedContent { get; set; } = new();
    public List<DeletionHistoryVM> DeletionHistory { get; set; } = new();
    public Dictionary<string, object> OriginalData { get; set; } = new();
}
