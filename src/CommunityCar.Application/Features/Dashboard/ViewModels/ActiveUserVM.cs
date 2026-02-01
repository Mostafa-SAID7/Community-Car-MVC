namespace CommunityCar.Application.Features.Dashboard.ViewModels;

public class ActiveUserVM
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public DateTime LastActivity { get; set; }
    public string LastActivityText { get; set; } = string.Empty;
    public int PostsCount { get; set; }
    public int CommentsCount { get; set; }
    public bool IsOnline { get; set; }
}