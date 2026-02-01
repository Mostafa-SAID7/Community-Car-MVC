namespace CommunityCar.Application.Features.Account.ViewModels.Management;

public class NotificationSettingsVM
{
    public bool EmailNotifications { get; set; }
    public bool PushNotifications { get; set; }
    public bool SmsNotifications { get; set; }
    public bool WeeklyDigest { get; set; }
    public bool MarketingEmails { get; set; }
    public bool SecurityAlerts { get; set; } = true;
}