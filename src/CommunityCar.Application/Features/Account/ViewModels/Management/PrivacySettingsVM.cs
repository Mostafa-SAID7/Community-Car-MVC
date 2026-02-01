namespace CommunityCar.Application.Features.Account.ViewModels.Management;

public class PrivacySettingsVM
{
    public bool IsPublic { get; set; } = true;
    public bool AllowMessages { get; set; }
    public bool AllowFriendRequests { get; set; }
    public string DefaultGalleryPrivacy { get; set; } = "public";
    public bool ShowActivityStatus { get; set; }
    public bool ShowOnlineStatus { get; set; }
    public bool ProfileVisible { get; set; } = true;
    public bool EmailVisible { get; set; }
    public bool PhoneVisible { get; set; }
}