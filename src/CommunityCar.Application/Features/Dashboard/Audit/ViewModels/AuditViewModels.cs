namespace CommunityCar.Application.Features.Dashboard.ViewModels;

public class AuditLogVM
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

public class AuditStatisticsVM
{
    public int TotalActions { get; set; }
    public int UniqueUsers { get; set; }
    public int SuccessfulActions { get; set; }
    public int FailedActions { get; set; }
    public string MostActiveUser { get; set; } = string.Empty;
    public string MostCommonAction { get; set; } = string.Empty;
    public string MostAffectedEntityType { get; set; } = string.Empty;
    public int AverageActionsPerDay { get; set; }
    public int PeakActivityHour { get; set; }
    public Dictionary<string, int> TopActions { get; set; } = new();
}

public class UserAuditSummaryVM
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public int TotalActions { get; set; }
    public int CreateActions { get; set; }
    public int UpdateActions { get; set; }
    public int DeleteActions { get; set; }
    public int ViewActions { get; set; }
    public DateTime LastActivity { get; set; }
}