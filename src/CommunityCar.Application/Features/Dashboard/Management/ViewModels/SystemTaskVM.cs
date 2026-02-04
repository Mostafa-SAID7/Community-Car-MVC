namespace CommunityCar.Application.Features.Dashboard.Management.ViewModels;

public class SystemTaskVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public decimal ProgressPercentage { get; set; }
    public string Result { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public int Priority { get; set; }
    public TimeSpan? EstimatedDuration { get; set; }
    public TimeSpan? ActualDuration { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
}

public class SystemLogVM
{
    public Guid Id { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Category { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public Dictionary<string, object> Properties { get; set; } = new();
    public string Exception { get; set; } = string.Empty;
}