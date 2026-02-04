namespace CommunityCar.Application.Features.Community.SoftDelete.ViewModels;

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