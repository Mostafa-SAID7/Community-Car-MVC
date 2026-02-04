using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.AI.ViewModels;

/// <summary>
/// ViewModel for performance testing requests
/// </summary>
public class PerformanceTestRequestVM
{
    /// <summary>
    /// Number of concurrent requests to make
    /// </summary>
    [Range(1, 100, ErrorMessage = "Concurrent requests must be between 1 and 100")]
    public int ConcurrentRequests { get; set; } = 5;
    
    /// <summary>
    /// Duration of the test in seconds
    /// </summary>
    [Range(10, 300, ErrorMessage = "Duration must be between 10 and 300 seconds")]
    public int DurationSeconds { get; set; } = 30;
    
    /// <summary>
    /// Optional test message to use (defaults to standard test message)
    /// </summary>
    [StringLength(1000, ErrorMessage = "Test message cannot exceed 1000 characters")]
    public string? TestMessage { get; set; }
}