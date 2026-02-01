namespace CommunityCar.Application.Features.Account.ViewModels.Authentication;

public class SessionManagementVM
{
    public Guid UserId { get; set; }
    public List<UserSessionVM> ActiveSessions { get; set; } = new();
    public List<UserSessionVM> RecentSessions { get; set; } = new();
    public int TotalActiveSessions { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string? LastLoginLocation { get; set; }
}