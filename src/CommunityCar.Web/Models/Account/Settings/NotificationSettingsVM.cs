namespace CommunityCar.Web.Models.Account.Settings;

public class NotificationSettingsVM
{
    public bool EmailNotifications { get; set; }
    public bool MarketingEmails { get; set; }
    public bool PushNotifications { get; set; }
    public bool SmsNotifications { get; set; }
}