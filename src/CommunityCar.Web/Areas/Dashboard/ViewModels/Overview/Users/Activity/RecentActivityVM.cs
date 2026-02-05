namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Overview.Users.Activity;

public class RecentActivityVM
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string ActivityType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? EntityId { get; set; }
    public string? EntityType { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}




