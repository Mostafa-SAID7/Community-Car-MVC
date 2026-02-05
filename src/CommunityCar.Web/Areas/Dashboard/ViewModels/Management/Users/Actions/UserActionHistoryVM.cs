namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.Users.Actions;

/// <summary>
/// User action history view model
/// </summary>
public class UserActionHistoryVM
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime ActionDate { get; set; }
    public string PerformedBy { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

/// <summary>
/// User action statistics view model
/// </summary>
public class UserActionStatsVM
{
    public int TotalActions { get; set; }
    public int ActionsToday { get; set; }
    public int ActionsThisWeek { get; set; }
    public int ActionsThisMonth { get; set; }
    public Dictionary<string, int> ActionsByType { get; set; } = new();
    public DateTime LastActionDate { get; set; }
}




