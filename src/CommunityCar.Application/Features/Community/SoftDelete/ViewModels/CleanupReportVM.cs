namespace CommunityCar.Application.Features.Community.SoftDelete.ViewModels;

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