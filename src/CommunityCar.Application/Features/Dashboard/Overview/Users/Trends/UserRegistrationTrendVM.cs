namespace CommunityCar.Application.Features.Dashboard.Overview.Users.Trends;

/// <summary>
/// User registration trend view model
/// </summary>
public class UserRegistrationTrendVM
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
    public string Label { get; set; } = string.Empty;
    public decimal GrowthRate { get; set; }
    public int CumulativeCount { get; set; }
}