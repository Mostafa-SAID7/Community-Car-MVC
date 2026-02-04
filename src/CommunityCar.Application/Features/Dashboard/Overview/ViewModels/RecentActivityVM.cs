namespace CommunityCar.Application.Features.Dashboard.Overview.ViewModels;

public class RecentActivityVM
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserAvatar { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime Timestamp { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}