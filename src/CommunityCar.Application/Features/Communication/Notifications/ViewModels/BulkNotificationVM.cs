using CommunityCar.Domain.Enums.Account;

namespace CommunityCar.Application.Features.Communication.Notifications.ViewModels;

public class BulkNotificationVM
{
    public List<Guid> UserIds { get; set; } = new();
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public string? ActionUrl { get; set; }
    public string? IconClass { get; set; }
    public Dictionary<string, object>? Data { get; set; }
}