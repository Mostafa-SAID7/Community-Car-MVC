namespace CommunityCar.Application.Features.Shared.ViewModels;

public class SharedCommentVM
{
    public Guid Id { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorAvatar { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string TimeAgo { get; set; } = string.Empty;
    public int LikeCount { get; set; }
    public bool CanReply { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
}