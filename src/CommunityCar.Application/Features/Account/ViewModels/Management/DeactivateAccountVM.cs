namespace CommunityCar.Application.Features.Account.ViewModels.Management;

public class DeactivateAccountVM
{
    public string Reason { get; set; } = string.Empty;
    public bool ConfirmDeactivation { get; set; }
}

public class DeleteAccountVM
{
    public string Password { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public bool ConfirmDeletion { get; set; }
}

public class ExportDataVM
{
    public string Format { get; set; } = "JSON";
    public bool IncludePersonalData { get; set; } = true;
    public bool IncludeActivityData { get; set; } = true;
}

public class PrivacySettingsVM
{
    public bool IsPublic { get; set; }
    public bool ShowEmail { get; set; }
    public bool ShowLocation { get; set; }
}

public class NotificationSettingsVM
{
    public bool EmailNotifications { get; set; }
    public bool PushNotifications { get; set; }
    public bool SmsNotifications { get; set; }
}