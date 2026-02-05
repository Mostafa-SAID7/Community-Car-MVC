namespace CommunityCar.Application.Features.Dashboard.Analytics.Users.Behavior;

public class AnalyticsInterestVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Category { get; set; } = string.Empty;
    public string SubCategory { get; set; } = string.Empty;
    public string InterestType { get; set; } = string.Empty;
    public string InterestValue { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public double Score { get; set; }
    public int InteractionCount { get; set; }
    public DateTime LastInteraction { get; set; }
    public string? Source { get; set; }
    public bool IsActive { get; set; }
    public string ScoreLevel { get; set; } = string.Empty; // Low, Medium, High, Very High
}