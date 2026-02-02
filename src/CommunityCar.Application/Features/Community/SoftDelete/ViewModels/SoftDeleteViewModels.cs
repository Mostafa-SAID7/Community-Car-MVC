namespace CommunityCar.Application.Features.Community.SoftDelete.ViewModels;

/// <summary>
/// Request model for soft delete operations
/// </summary>
public class SoftDeleteRequestVM
{
    public Guid Id { get; set; }
    public string? Reason { get; set; }
    public bool NotifyUser { get; set; } = false;
}

/// <summary>
/// Request model for bulk soft delete operations
/// </summary>
public class BulkSoftDeleteRequestVM
{
    public IEnumerable<Guid> Ids { get; set; } = new List<Guid>();
    public string? Reason { get; set; }
    public bool NotifyUsers { get; set; } = false;
}

/// <summary>
/// Response model for soft delete operations
/// </summary>
public class SoftDeleteResponseVM
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? Id { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}

/// <summary>
/// Response model for bulk operations
/// </summary>
public class BulkSoftDeleteResponseVM
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int TotalRequested { get; set; }
    public int TotalProcessed { get; set; }
    public int TotalSuccessful { get; set; }
    public int TotalFailed { get; set; }
    public IEnumerable<Guid> SuccessfulIds { get; set; } = new List<Guid>();
    public IEnumerable<Guid> FailedIds { get; set; } = new List<Guid>();
    public IEnumerable<string> Errors { get; set; } = new List<string>();
}

/// <summary>
/// Request model for restore operations
/// </summary>
public class RestoreRequestVM
{
    public Guid Id { get; set; }
    public string? Reason { get; set; }
    public bool NotifyUser { get; set; } = false;
}

/// <summary>
/// Request model for bulk restore operations
/// </summary>
public class BulkRestoreRequestVM
{
    public IEnumerable<Guid> Ids { get; set; } = new List<Guid>();
    public string? Reason { get; set; }
    public bool NotifyUsers { get; set; } = false;
}

/// <summary>
/// Response model for restore operations
/// </summary>
public class RestoreResponseVM
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? Id { get; set; }
    public DateTime? RestoredAt { get; set; }
    public string? RestoredBy { get; set; }
}

/// <summary>
/// Request model for permanent delete operations
/// </summary>
public class PermanentDeleteRequestVM
{
    public Guid Id { get; set; }
    public string? Reason { get; set; }
    public bool ConfirmPermanentDelete { get; set; } = false;
}

/// <summary>
/// Response model for permanent delete operations
/// </summary>
public class PermanentDeleteResponseVM
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? Id { get; set; }
    public DateTime? PermanentlyDeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}

/// <summary>
/// Filter model for deleted content search
/// </summary>
public class DeletedContentFilterVM
{
    public string? SearchTerm { get; set; }
    public string? ContentType { get; set; } // Post, Story, Group, Comment
    public Guid? AuthorId { get; set; }
    public DateTime? DeletedAfter { get; set; }
    public DateTime? DeletedBefore { get; set; }
    public string? DeletedBy { get; set; }
    public string? SortBy { get; set; } = "DeletedAt";
    public bool SortDescending { get; set; } = true;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

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

/// <summary>
/// Deleted content search results
/// </summary>
public class DeletedContentSearchVM
{
    public IEnumerable<DeletedContentItemVM> Items { get; set; } = new List<DeletedContentItemVM>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
    public DeletedContentFilterVM Filter { get; set; } = new();
}

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

/// <summary>
/// Cleanup report model
/// </summary>
public class CleanupReportVM
{
    public DateTime CleanupDate { get; set; }
    public string PerformedBy { get; set; } = string.Empty;
    public int TotalItemsEvaluated { get; set; }
    public int TotalItemsCleaned { get; set; }
    public Dictionary<string, int> CleanedByContentType { get; set; } = new();
    public TimeSpan Duration { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public IEnumerable<string> Warnings { get; set; } = new List<string>();
}

/// <summary>
/// User content deletion summary
/// </summary>
public class UserContentDeletionSummaryVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public int TotalPosts { get; set; }
    public int DeletedPosts { get; set; }
    public int TotalStories { get; set; }
    public int DeletedStories { get; set; }
    public int TotalGroups { get; set; }
    public int DeletedGroups { get; set; }
    public int TotalComments { get; set; }
    public int DeletedComments { get; set; }
    public DateTime? LastDeletionDate { get; set; }
    public string? LastDeletionBy { get; set; }
    public bool HasActiveContent => (TotalPosts - DeletedPosts) > 0 || 
                                   (TotalStories - DeletedStories) > 0 || 
                                   (TotalGroups - DeletedGroups) > 0 || 
                                   (TotalComments - DeletedComments) > 0;
}