namespace CommunityCar.Application.Features.Shared.Interactions.ViewModels;

public class CommentVM
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorAvatar { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
    public bool IsEdited { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public Guid? ParentCommentId { get; set; }
    public List<CommentVM> Replies { get; set; } = new();
    public int ReplyCount { get; set; }
    public int LikeCount { get; set; }
    public ReactionSummaryVM Reactions { get; set; } = new();
}