using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Models.Profile.Settings;

public class PrivacySettingsVM
{
    public bool AllowMessages { get; set; }
    public bool AllowFriendRequests { get; set; }
    public string DefaultGalleryPrivacy { get; set; } = "public";
    public bool ShowActivityStatus { get; set; }
    
    // Compatibility fields if needed
    public bool ProfileVisible { get; set; } = true;
    public bool EmailVisible { get; set; }
    public bool PhoneVisible { get; set; }
}