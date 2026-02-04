using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Features.Dashboard.Management.ViewModels;

public class ContentModerationVM
{
    public Guid Id { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ReportedAt { get; set; }
    public string Status { get; set; } = string.Empty; // Pending, Approved, Rejected, Removed
    public string Priority { get; set; } = string.Empty; // Low, Medium, High, Critical
    public int ReportCount { get; set; }
    public List<string> ReportReasons { get; set; } = new();
    public string? ModeratorNotes { get; set; }
    public string? ModeratedBy { get; set; }
    public DateTime? ModeratedAt { get; set; }
    public List<ChartDataVM> ModerationStats { get; set; } = new();
}