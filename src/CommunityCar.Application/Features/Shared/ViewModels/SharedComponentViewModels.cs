namespace CommunityCar.Application.Features.Shared.ViewModels;

public class StatsBoxVM
{
    public string Type { get; set; } = string.Empty; // comments, likes, views, shares, bookmarks
    public int Count { get; set; }
    public string EntityId { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public bool AlwaysShow { get; set; } = false;
    public bool IsActive { get; set; } = false;
}

public class ActionButtonVM
{
    public string Type { get; set; } = string.Empty; // like, comment, share, bookmark, view
    public string Action { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public bool IsActive { get; set; } = false;
    public string OnClick { get; set; } = string.Empty;
}

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

public class ActionToolbarVM
{
    public string EntityId { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public bool IsLikedByUser { get; set; }
    public bool IsBookmarkedByUser { get; set; }
}

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