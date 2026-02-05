namespace CommunityCar.Application.Features.Shared.ViewModels;

/// <summary>
/// General statistics view model
/// </summary>
public class StatsVM
{
    public string Title { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Change { get; set; } = string.Empty;
    public string ChangeType { get; set; } = "positive"; // positive, negative, neutral
    public string Description { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();

    // Dashboard specific properties
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalContent { get; set; }
    public int TotalEngagements { get; set; }
}