namespace CommunityCar.Application.Features.Community.SoftDelete.ViewModels;

/// <summary>
/// Cleanup configuration model
/// </summary>
public class CleanupConfigurationVM
{
    public int DaysToKeepDeleted { get; set; } = 30;
    public bool AutoCleanupEnabled { get; set; } = false;
    public int AutoCleanupIntervalDays { get; set; } = 7;
    public bool NotifyBeforeCleanup { get; set; } = true;
    public int NotificationDaysBefore { get; set; } = 3;
    public IEnumerable<string> ContentTypesToCleanup { get; set; } = new List<string> { "Post", "Story", "Comment" };
    public IEnumerable<string> ExcludedContentTypes { get; set; } = new List<string> { "Group" };
}