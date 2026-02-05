using CommunityCar.Application.Features.Dashboard.Management.Content.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.Management.Content;

/// <summary>
/// Service for managing content moderation in the dashboard
/// </summary>
public interface IContentModerationService
{
    /// <summary>
    /// Get all pending moderation items
    /// </summary>
    Task<List<ModerationItemVM>> GetPendingModerationItemsAsync(int page = 1, int pageSize = 20);

    /// <summary>
    /// Get recently moderated items
    /// </summary>
    Task<List<ModerationItemVM>> GetRecentlyModeratedItemsAsync(int page = 1, int pageSize = 20);

    /// <summary>
    /// Get moderation dashboard overview
    /// </summary>
    Task<ContentModerationVM> GetModerationDashboardAsync();

    /// <summary>
    /// Moderate content (approve, reject, delete)
    /// </summary>
    Task<bool> ModerateContentAsync(ModerateContentVM request, Guid moderatorId);

    /// <summary>
    /// Get moderation item details
    /// </summary>
    Task<ModerationItemVM?> GetModerationItemAsync(Guid contentId, string contentType);

    /// <summary>
    /// Filter toxic content from text
    /// </summary>
    Task<string> FilterToxicContentAsync(string content);

    /// <summary>
    /// Check if content contains toxic words
    /// </summary>
    Task<bool> IsContentToxicAsync(string content);

    /// <summary>
    /// Get moderation statistics
    /// </summary>
    Task<ModerationStatsVM> GetModerationStatsAsync(DateTime? startDate = null, DateTime? endDate = null);
}

public class ModerationStatsVM
{
    public int PendingItems { get; set; }
    public int ApprovedToday { get; set; }
    public int RejectedToday { get; set; }
    public int DeletedToday { get; set; }
    public int TotalReports { get; set; }
    public int AutoFlaggedItems { get; set; }
    public double AverageResponseTime { get; set; }
}
