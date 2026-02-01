namespace CommunityCar.Application.Features.Shared.ViewModels;

public class StatsOverlayVM
{
    public string EntityId { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public int CommentCount { get; set; }
    public int LikeCount { get; set; }
    public int ViewCount { get; set; }
    public int ShareCount { get; set; }
    public bool IsLikedByUser { get; set; }
}