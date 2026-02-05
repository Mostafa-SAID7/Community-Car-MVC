namespace CommunityCar.Application.Features.Dashboard.Overview.ViewModels;

public class ActiveUserVM
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public DateTime LastActivity { get; set; }
    public string Status { get; set; } = string.Empty;
    public int SessionCount { get; set; }
    public TimeSpan OnlineTime { get; set; }
    public string CurrentPage { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Device { get; set; } = string.Empty;
    public bool IsOnline { get; set; }
}