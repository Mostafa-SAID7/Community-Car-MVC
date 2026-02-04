namespace CommunityCar.Application.Common.Interfaces.Services.Community.Moderation;

public interface IContentModerationService
{
    Task<bool> GetContentModerationAsync();
    Task<bool> ModerateContentAsync(Guid contentId, string contentType, string action, string reason, Guid moderatorId, CancellationToken cancellationToken = default);
    Task<List<ModerationItemVM>> GetPendingModerationAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<bool> ApproveContentAsync(Guid contentId, string contentType, Guid moderatorId, CancellationToken cancellationToken = default);
    Task<bool> RejectContentAsync(Guid contentId, string contentType, string reason, Guid moderatorId, CancellationToken cancellationToken = default);
    Task<ModerationStatsVM> GetModerationStatsAsync(CancellationToken cancellationToken = default);
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