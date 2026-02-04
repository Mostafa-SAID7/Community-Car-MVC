namespace CommunityCar.Application.Features.Shared.Interactions.ViewModels;

public class InteractionVM
{
    public Guid Id { get; set; }
    public Guid EntityId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string InteractionType { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Content { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class InteractionStatsVM
{
    public Guid EntityId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public int TotalInteractions { get; set; }
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    public int SharesCount { get; set; }
    public int ViewsCount { get; set; }
    public int BookmarksCount { get; set; }
    public decimal EngagementRate { get; set; }
    public DateTime LastInteraction { get; set; }
    public Dictionary<string, int> InteractionsByType { get; set; } = new();
}