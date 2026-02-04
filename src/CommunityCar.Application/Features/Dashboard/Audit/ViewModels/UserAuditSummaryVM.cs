namespace CommunityCar.Application.Features.Dashboard.Audit.ViewModels;

public class UserAuditSummaryVM
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int TotalActions { get; set; }
    public int CreateActions { get; set; }
    public int UpdateActions { get; set; }
    public int DeleteActions { get; set; }
    public DateTime FirstActivity { get; set; }
    public DateTime LastActivity { get; set; }
    public List<string> MostCommonActions { get; set; } = new();
    public Dictionary<string, int> ActionsByType { get; set; } = new();
}