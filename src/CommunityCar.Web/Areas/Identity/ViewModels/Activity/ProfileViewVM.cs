namespace CommunityCar.Web.Areas.Identity.ViewModels.Activity;

public class ProfileViewVM
{
    public Guid Id { get; set; }
    public Guid ViewerId { get; set; }
    public Guid ProfileUserId { get; set; }
    public string ViewerName { get; set; } = string.Empty;
    public string? ViewerProfilePicture { get; set; }
    public string? ViewerLocation { get; set; }
    public string? Location => ViewerLocation;
    public DateTime ViewedAt { get; set; }
    public string ViewedTimeAgo { get; set; } = string.Empty;
    public string TimeAgo => ViewedTimeAgo;
    public bool IsAnonymous { get; set; }
    public string? Device { get; set; } 
    public string? DeviceType => Device;
    public string? ViewSource { get; set; }
    public bool IsMutual { get; set; }
}
