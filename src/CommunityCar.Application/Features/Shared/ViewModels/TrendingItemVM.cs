using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Shared.ViewModels;

public class TrendingItemVM
{
    public Guid Id { get; set; }
    public EntityType EntityType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int InteractionCount { get; set; }
    public int ViewCount { get; set; }
    public double TrendingScore { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Tags { get; set; } = new();
    
    // Missing property that services expect
    public int Count { get; set; }
}