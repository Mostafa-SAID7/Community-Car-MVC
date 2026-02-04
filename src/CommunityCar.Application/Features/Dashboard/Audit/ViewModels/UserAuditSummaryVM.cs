namespace CommunityCar.Application.Features.Dashboard.Audit.ViewModels;

/// <summary>
/// ViewModel for user audit summary
/// </summary>
public class UserAuditSummaryVM
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int TotalActions { get; set; }
    public int CreateActions { get; set; }
    public int UpdateActions { get; set; }
    public int DeleteActions { get; set; }
    public int ViewActions { get; set; }
    public int SuccessfulActions { get; set; }
    public int FailedActions { get; set; }
    public DateTime LastActivity { get; set; }
    public DateTime FirstActivity { get; set; }
    public Dictionary<string, int> ActionBreakdown { get; set; } = new();
    public List<string> MostCommonActions { get; set; } = new();
    public double SuccessRate => TotalActions > 0 ? (double)SuccessfulActions / TotalActions * 100 : 0;
}