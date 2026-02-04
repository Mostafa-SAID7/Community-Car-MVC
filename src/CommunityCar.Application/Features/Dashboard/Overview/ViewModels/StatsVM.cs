namespace CommunityCar.Application.Features.Dashboard.Overview.ViewModels;

public class StatsVM
{
    public string Title { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Change { get; set; } = string.Empty;
    public string ChangeType { get; set; } = string.Empty; // increase, decrease, neutral
    public string Description { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
}