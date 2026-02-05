namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.Users.Actions;

/// <summary>
/// ViewModel for user activity summary
/// </summary>
public class UserActivitySummaryVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public DateTime LastActivity { get; set; }
    public string LastAction { get; set; } = string.Empty;
    public int TotalActions { get; set; }
    public int ActionsToday { get; set; }
    public int LoginCount { get; set; }
    public TimeSpan TotalTimeSpent { get; set; }
    public string Status { get; set; } = string.Empty; // Online, Away, Offline
    public string ActivityLevel { get; set; } = string.Empty; // Low, Medium, High
    
    // Missing properties that services expect
    public int TotalActivities { get; set; }
    public List<string> MostActiveUsers { get; set; } = new();
    public Dictionary<string, int> ActivityByType { get; set; } = new();
}




