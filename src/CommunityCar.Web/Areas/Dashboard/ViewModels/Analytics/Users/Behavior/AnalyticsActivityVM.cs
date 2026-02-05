using CommunityCar.Domain.Enums.Account;

namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Analytics.Users.Behavior;

public class AnalyticsActivityVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public ActivityType ActivityType { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public string? EntityTitle { get; set; }
    public string? Description { get; set; }
    public DateTime ActivityDate { get; set; }
    public int Duration { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
    public string ActivityIcon { get; set; } = string.Empty;
    public string ActivityColor { get; set; } = string.Empty;
}




