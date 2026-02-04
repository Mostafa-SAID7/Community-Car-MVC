namespace CommunityCar.Application.Features.Community.SoftDelete.ViewModels;

public class DeletedContentSummaryVM
{
    public Guid Id { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public DateTime DeletedAt { get; set; }
    public string DeletedBy { get; set; } = string.Empty;
    public string DeleteReason { get; set; } = string.Empty;
    public bool CanRestore { get; set; }
    public DateTime? PermanentDeletionDate { get; set; }
    public int DaysUntilPermanentDeletion { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}