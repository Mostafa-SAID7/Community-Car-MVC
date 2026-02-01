namespace CommunityCar.Application.Features.Community.Feed.ViewModels;

/// <summary>
/// Comment item view model
/// </summary>
public class CommentItemVM
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorAvatar { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int LikeCount { get; set; }
    public bool IsLiked { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
}