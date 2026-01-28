using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Dashboard.Settings;

public class NotificationSetting : BaseEntity
{
    public Guid UserId { get; set; }
    public string NotificationType { get; set; } = string.Empty;
    public bool EmailEnabled { get; set; }
    public bool PushEnabled { get; set; }
    public bool SmsEnabled { get; set; }
    public bool InAppEnabled { get; set; }
    public string? Schedule { get; set; } // JSON for scheduling rules
    public Dictionary<string, object> Preferences { get; set; } = new();

    public void UpdatePreferences(bool email, bool push, bool sms, bool inApp, string? schedule = null)
    {
        EmailEnabled = email;
        PushEnabled = push;
        SmsEnabled = sms;
        InAppEnabled = inApp;
        Schedule = schedule;
        Audit(UpdatedBy);
    }
}