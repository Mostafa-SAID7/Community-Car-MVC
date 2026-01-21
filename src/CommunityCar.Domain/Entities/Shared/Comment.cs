using System;
using System.Collections.Generic;
using CommunityCar.Domain.Base;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Domain.Entities.Shared;

public class Comment : BaseEntity
{
    public string Content { get; private set; }
    public Guid EntityId { get; private set; } // ID of Post, Question, etc.
    public EntityType EntityType { get; private set; }
    public Guid AuthorId { get; private set; }
    
    // Support for nested comments
    public Guid? ParentCommentId { get; private set; }
    private readonly List<Comment> _replies = new();
    public IReadOnlyCollection<Comment> Replies => _replies.AsReadOnly();

    public Comment(string content, Guid entityId, EntityType entityType, Guid authorId, Guid? parentCommentId = null)
    {
        Content = content;
        EntityId = entityId;
        EntityType = entityType;
        AuthorId = authorId;
        ParentCommentId = parentCommentId;
    }

    private Comment() { }

    public void UpdateContent(string newContent)
    {
        Content = newContent;
        Audit(UpdatedBy);
    }

    public void AddReply(Comment reply)
    {
        _replies.Add(reply);
    }
}
