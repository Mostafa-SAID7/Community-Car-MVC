using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Shared;

public class ErrorStats : BaseEntity
{
    public DateTime Date { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public int TotalErrors { get; set; }
    public int ResolvedErrors { get; set; }
    public int UnresolvedErrors { get; set; }
    public int CriticalErrors { get; set; }
    public int WarningErrors { get; set; }
    public int InfoErrors { get; set; }
    public string? MostCommonError { get; set; }
    public int MostCommonErrorCount { get; set; }
    public double AverageResolutionTime { get; set; } // in hours
    public string? TopErrorSource { get; set; }
    public int UniqueUsers { get; set; }
    public int TotalOccurrences { get; set; }
}

public class ErrorBoundary : BaseEntity
{
    public string BoundaryName { get; set; } = string.Empty;
    public string ComponentName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
    public string? ComponentStack { get; set; }
    public string? Props { get; set; } // JSON string
    public string? State { get; set; } // JSON string
    public string? UserAgent { get; set; }
    public string? Url { get; set; }
    public string? UserId { get; set; }
    public bool IsRecovered { get; set; } = false;
    public DateTime? RecoveredAt { get; set; }
    public string? RecoveryAction { get; set; }
    public int OccurrenceCount { get; set; } = 1;
    public DateTime LastOccurrence { get; set; } = DateTime.UtcNow;
}