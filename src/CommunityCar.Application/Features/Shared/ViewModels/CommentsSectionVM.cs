namespace CommunityCar.Application.Features.Shared.ViewModels;

public class CommentsSectionVM
{
    public string EntityId { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public bool CanComment { get; set; }
    public string CurrentUserAvatar { get; set; } = string.Empty;
    public string CommentPlaceholder { get; set; } = "Write a comment...";
    public string NoCommentsMessage { get; set; } = "No comments yet. Be the first to comment!";
    public List<SharedCommentVM> Comments { get; set; } = new();
    public bool HasMoreComments { get; set; }
    public int TotalComments { get; set; }
}