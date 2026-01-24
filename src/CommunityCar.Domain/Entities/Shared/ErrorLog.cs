using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Shared;

public class ErrorLog : BaseEntity
{
    public string ErrorId { get; set; } = Guid.NewGuid().ToString();
    public string Message { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
    public string? InnerException { get; set; }
    public string Source { get; set; } = string.Empty;
    public string Severity { get; set; } = "Error"; // Error, Warning, Critical, Info
    public string Category { get; set; } = "General"; // Database, Network, Validation, Business, Security
    public string? UserId { get; set; }
    public string? UserAgent { get; set; }
    public string? IpAddress { get; set; }
    public string? RequestPath { get; set; }
    public string? RequestMethod { get; set; }
    public string? RequestHeaders { get; set; }
    public string? RequestBody { get; set; }
    public int? StatusCode { get; set; }
    public string? AdditionalData { get; set; } // JSON string for extra context
    public bool IsResolved { get; set; } = false;
    public DateTime? ResolvedAt { get; set; }
    public string? ResolvedBy { get; set; }
    public string? Resolution { get; set; }
    public int OccurrenceCount { get; set; } = 1;
    public DateTime LastOccurrence { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ICollection<ErrorOccurrence> Occurrences { get; set; } = new List<ErrorOccurrence>();
}

public class ErrorOccurrence : BaseEntity
{
    public Guid ErrorLogId { get; set; }
    public string? UserId { get; set; }
    public string? SessionId { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? RequestPath { get; set; }
    public string? AdditionalContext { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ErrorLog ErrorLog { get; set; } = null!;
}