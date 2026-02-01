namespace CommunityCar.Application.Features.Dashboard.ViewModels;

public class ErrorBoundaryVM
{
    public string Id { get; set; } = string.Empty;
    public string BoundaryName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public bool IsRecovered { get; set; }
    public string? RecoveryAction { get; set; }
    public DateTime? RecoveredAt { get; set; }
    public int FailureCount { get; set; }
}