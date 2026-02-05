namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Overview.Users.Activity;

/// <summary>
/// Active user view model for overview
/// </summary>
public class ActiveUserVM
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public DateTime LastActivity { get; set; }
    public bool IsOnline { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}




