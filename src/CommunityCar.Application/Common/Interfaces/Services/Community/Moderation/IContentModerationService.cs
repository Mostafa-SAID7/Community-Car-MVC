namespace CommunityCar.Application.Common.Interfaces.Services.Community.Moderation;

public interface IContentModerationService
{
    Task<string> FilterToxicContentAsync(string content);
    Task<bool> IsContentToxicAsync(string content);
}

public class ModerationItemVM
{
    public Guid Id { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public int ReportCount { get; set; }
}

public class ModerationStatsVM
{
    public int PendingItems { get; set; }
    public int ApprovedToday { get; set; }
    public int RejectedToday { get; set; }
    public int TotalReports { get; set; }
}