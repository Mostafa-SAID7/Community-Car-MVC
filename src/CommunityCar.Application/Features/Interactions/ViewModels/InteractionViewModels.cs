using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Features.Interactions.ViewModels;

public class ReactionVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? UserAvatar { get; set; }
    public ReactionType Type { get; set; }
    public string TypeDisplay { get; set; } = string.Empty;
    public string TypeIcon { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
}

public class ReactionSummaryVM
{
    public Dictionary<ReactionType, int> ReactionCounts { get; set; } = new();
    public int TotalReactions { get; set; }
    public ReactionType? UserReaction { get; set; }
    public bool HasUserReacted { get; set; }
    public List<ReactionTypeInfoVM> AvailableReactions { get; set; } = new();
}

public class ReactionTypeInfoVM
{
    public ReactionType Type { get; set; }
    public string Display { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class ReactionResultVM
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public ReactionSummaryVM? Summary { get; set; }
}

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
    public ReactionSummaryVM Reactions { get; set; } = new();
}

public class ShareVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? UserAvatar { get; set; }
    public ShareType ShareType { get; set; }
    public string ShareTypeDisplay { get; set; } = string.Empty;
    public string? ShareMessage { get; set; }
    public string? Platform { get; set; }
    public DateTime CreatedAt { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
}

public class ShareSummaryVM
{
    public int TotalShares { get; set; }
    public Dictionary<ShareType, int> ShareTypeCounts { get; set; } = new();
    public bool HasUserShared { get; set; }
    public string ShareUrl { get; set; } = string.Empty;
}

public class ShareResultVM
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? ShareUrl { get; set; }
    public ShareSummaryVM? Summary { get; set; }
}

public class ShareMetadataVM
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public Dictionary<string, string> SocialMediaUrls { get; set; } = new();
}

public class InteractionSummaryVM
{
    public ReactionSummaryVM Reactions { get; set; } = new();
    public int CommentCount { get; set; }
    public ShareSummaryVM Shares { get; set; } = new();
    public bool CanComment { get; set; }
    public bool CanShare { get; set; }
    public bool CanReact { get; set; }
}