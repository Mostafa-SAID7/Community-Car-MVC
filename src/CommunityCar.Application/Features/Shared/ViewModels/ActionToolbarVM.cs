namespace CommunityCar.Application.Features.Shared.ViewModels;

public class ActionToolbarVM
{
    public string EntityId { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public bool IsLikedByUser { get; set; }
    public bool IsBookmarkedByUser { get; set; }
}