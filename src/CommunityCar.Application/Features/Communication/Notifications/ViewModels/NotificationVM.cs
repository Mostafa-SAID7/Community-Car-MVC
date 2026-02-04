using CommunityCar.Domain.Enums.Account;

namespace CommunityCar.Application.Features.Notifications.ViewModels;

public class NotificationVM
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public string? ActionUrl { get; set; }
    public string? IconClass { get; set; }
    public Dictionary<string, object>? Data { get; set; }
}