namespace CommunityCar.Application.Features.Account.ViewModels.Management;

public class UpdateNotificationSettingsRequest : NotificationSettingsVM
{
    public Guid UserId { get; set; }
}