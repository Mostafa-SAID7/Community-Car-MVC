namespace CommunityCar.Application.Features.Dashboard.Moderation.ViewModels;

public class ModerationItemVM
{
    public Guid Id { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorEmail { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime SubmittedForModerationAt { get; set; }
    public string Status { get; set; } = "Pending";
    public string ReasonForReporting { get; set; } = string.Empty;
    public int ReportCount { get; set; }
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}