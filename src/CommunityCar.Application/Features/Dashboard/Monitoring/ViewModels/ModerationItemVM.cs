namespace CommunityCar.Application.Features.Dashboard.Monitoring.ViewModels;

/// <summary>
/// ViewModel for content moderation items
/// </summary>
public class ModerationItemVM
{
    public Guid Id { get; set; }
    public string ContentType { get; set; } = string.Empty; // Post, Comment, User, etc.
    public Guid ContentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public string ReportReason { get; set; } = string.Empty;
    public string ReporterName { get; set; } = string.Empty;
    public Guid ReporterId { get; set; }
    public DateTime ReportedDate { get; set; }
    public string Status { get; set; } = string.Empty; // Pending, Approved, Rejected, Escalated
    public string? ModeratorName { get; set; }
    public Guid? ModeratorId { get; set; }
    public DateTime? ModeratedDate { get; set; }
    public string? ModerationNotes { get; set; }
    public int Priority { get; set; } = 1; // 1-5, 5 being highest
    public List<string> Tags { get; set; } = new();
    public bool IsEscalated { get; set; }
}