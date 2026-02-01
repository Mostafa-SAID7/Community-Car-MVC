namespace CommunityCar.Application.Features.Account.ViewModels.Management;

public class UpdatePrivacySettingsRequest : PrivacySettingsVM
{
    public Guid UserId { get; set; }
}