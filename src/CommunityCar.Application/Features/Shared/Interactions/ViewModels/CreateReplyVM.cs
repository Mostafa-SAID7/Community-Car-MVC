namespace CommunityCar.Application.Features.Shared.Interactions.ViewModels;

public class CreateReplyVM
{
    public Guid CommentId { get; set; }
    public Guid ParentCommentId { get; set; }
    public Guid AuthorId { get; set; }
    public string Content { get; set; } = string.Empty;
}