using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.AI.ViewModels;

public class TestSentimentRequestVM
{
    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public string Text { get; set; } = string.Empty;
}

public class TestBatchSentimentRequestVM
{
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public List<string> Texts { get; set; } = new();
}

public class TestModerationRequestVM
{
    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public string Text { get; set; } = string.Empty;
}

public class TestSuggestionsRequestVM
{
    [Required]
    [StringLength(500, MinimumLength = 1)]
    public string Text { get; set; } = string.Empty;

    [Range(1, 20)]
    public int MaxSuggestions { get; set; } = 5;
}

public class TestPerformanceRequestVM
{
    [Required]
    [StringLength(500, MinimumLength = 1)]
    public string TestMessage { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Context { get; set; }

    [Range(1, 100)]
    public int Iterations { get; set; } = 10;
}

/// <summary>
/// ViewModel for batch testing requests
/// </summary>
public class BatchTestRequestVM
{
    /// <summary>
    /// List of test messages to process
    /// </summary>
    [Required(ErrorMessage = "Test messages are required")]
    [MinLength(1, ErrorMessage = "At least one test message is required")]
    public List<BatchTestMessage> TestMessages { get; set; } = new();
}

/// <summary>
/// Individual test message for batch testing
/// </summary>
public class BatchTestMessage
{
    /// <summary>
    /// The test message content
    /// </summary>
    [Required(ErrorMessage = "Message is required")]
    [StringLength(2000, ErrorMessage = "Message cannot exceed 2000 characters")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Optional context for the message
    /// </summary>
    [StringLength(1000, ErrorMessage = "Context cannot exceed 1000 characters")]
    public string? Context { get; set; }
}

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

/// <summary>
/// ViewModel for AI model validation requests
/// </summary>
public class ModelValidationRequestVM
{
    /// <summary>
    /// Name of the model to validate
    /// </summary>
    [Required(ErrorMessage = "Model name is required")]
    [StringLength(100, ErrorMessage = "Model name cannot exceed 100 characters")]
    public string ModelName { get; set; } = string.Empty;

    /// <summary>
    /// Validation test cases to run
    /// </summary>
    [Required(ErrorMessage = "Test cases are required")]
    [MinLength(1, ErrorMessage = "At least one test case is required")]
    public List<ValidationTestCase> TestCases { get; set; } = new();
}

/// <summary>
/// Individual validation test case
/// </summary>
public class ValidationTestCase
{
    /// <summary>
    /// Input for the test case
    /// </summary>
    [Required(ErrorMessage = "Input is required")]
    [StringLength(1000, ErrorMessage = "Input cannot exceed 1000 characters")]
    public string Input { get; set; } = string.Empty;

    /// <summary>
    /// Expected output or pattern
    /// </summary>
    [Required(ErrorMessage = "Expected output is required")]
    [StringLength(2000, ErrorMessage = "Expected output cannot exceed 2000 characters")]
    public string ExpectedOutput { get; set; } = string.Empty;

    /// <summary>
    /// Test case category
    /// </summary>
    [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
    public string Category { get; set; } = "General";

    /// <summary>
    /// Test case description
    /// </summary>
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }
}