using CommunityCar.Domain.Enums.Users;

namespace CommunityCar.Application.Common.Models.Notifications;

public class NotificationRequest
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public string? ActionUrl { get; set; }
    public string? IconClass { get; set; }
    public Dictionary<string, object>? Data { get; set; }
}

public class BulkNotificationRequest
{
    public List<Guid> UserIds { get; set; } = new();
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public string? ActionUrl { get; set; }
    public string? IconClass { get; set; }
    public Dictionary<string, object>? Data { get; set; }
}


