using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Features.Interactions.DTOs;

public class CreateCommentRequest
{
    public string Content { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public Guid AuthorId { get; set; }
    public Guid? ParentCommentId { get; set; }
}

public class CreateReplyRequest
{
    public string Content { get; set; } = string.Empty;
    public Guid ParentCommentId { get; set; }
    public Guid AuthorId { get; set; }
}

public class ShareEntityRequest
{
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public Guid UserId { get; set; }
    public ShareType ShareType { get; set; }
    public string? ShareMessage { get; set; }
    public string? Platform { get; set; }
}

public class UpdateCommentRequest
{
    public Guid CommentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}

public class DeleteCommentRequest
{
    public Guid CommentId { get; set; }
    public Guid UserId { get; set; }
}

public class ReactionRequest
{
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public Guid UserId { get; set; }
    public ReactionType ReactionType { get; set; }
}

public class UpdateCommentContentRequest
{
    public string Content { get; set; } = string.Empty;
}


