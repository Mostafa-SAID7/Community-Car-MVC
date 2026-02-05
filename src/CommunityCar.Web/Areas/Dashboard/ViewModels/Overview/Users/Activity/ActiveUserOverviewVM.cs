namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Overview.Users.Activity;

/// <summary>
/// Active user overview view model
/// </summary>
public class ActiveUserOverviewVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public DateTime LastActivity { get; set; }
    public string ActivityStatus { get; set; } = string.Empty; // Online, Away, Offline
    public int SessionDuration { get; set; } // in minutes
    public string CurrentLocation { get; set; } = string.Empty;
    public int PostsToday { get; set; }
    public int CommentsToday { get; set; }
}




