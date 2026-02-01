namespace CommunityCar.Application.Features.Shared.ViewModels;

public class StatsBoxVM
{
    public string Type { get; set; } = string.Empty; // comments, likes, views, shares, bookmarks
    public int Count { get; set; }
    public string EntityId { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public bool AlwaysShow { get; set; } = false;
    public bool IsActive { get; set; } = false;
}