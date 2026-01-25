using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Profile.DTOs;

public class UpdatePrivacySettingsRequest
{
    public bool ProfileVisible { get; set; } = true;
    public bool EmailVisible { get; set; }
    public bool PhoneVisible { get; set; }
    public bool AllowMessages { get; set; } = true;
    public bool AllowFriendRequests { get; set; } = true;
}