namespace CommunityCar.Application.Features.Dashboard.Settings.ViewModels;

public class DashboardNotificationSettingsVM
{
    public bool EnableEmailNotifications { get; set; }
    public bool EnablePushNotifications { get; set; }
    public bool EnableSmsNotifications { get; set; }
    public bool NotifyOnNewUser { get; set; }
    public bool NotifyOnSecurityAlert { get; set; }
    public bool NotifyOnSystemError { get; set; }
    public bool NotifyOnMaintenanceMode { get; set; }
    public List<string> AdminEmails { get; set; } = new();
}