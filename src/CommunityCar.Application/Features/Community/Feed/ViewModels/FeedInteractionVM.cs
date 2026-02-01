namespace CommunityCar.Application.Features.Community.Feed.ViewModels;

public class FeedInteractionVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid TargetId { get; set; }
    public string TargetType { get; set; } = string.Empty;
    public string InteractionType { get; set; } = string.Empty; // Like, Comment, Share, etc.
    public DateTime CreatedAt { get; set; }
    public string? Content { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserAvatar { get; set; } = string.Empty;
    public string UserSlug { get; set; } = string.Empty;
}