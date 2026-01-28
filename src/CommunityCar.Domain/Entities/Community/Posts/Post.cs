using System;
using System.Collections.Generic;
using CommunityCar.Domain.Base;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Entities.Shared;

namespace CommunityCar.Domain.Entities.Community.Posts;

public class Post : AggregateRoot
{
    public string Title { get; private set; }
    public string Content { get; private set; }
    
    // Arabic Localization
    public string? TitleAr { get; private set; }
    public string? ContentAr { get; private set; }
    
    public PostType Type { get; private set; }
    public Guid AuthorId { get; private set; }
    public Guid? CategoryId { get; private set; }
    
    // Publishing and visibility
    public bool IsPublished { get; private set; }
    public bool IsFeatured { get; private set; }
    
    // Engagement metrics
    public int ViewCount { get; private set; }
    public int LikeCount { get; private set; }
    public int CommentCount { get; private set; }
    
    // Navigation properties
    private readonly List<Tag> _tags = new();
    public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();
    
    // Simple navigation property placeholder - replace with actual relationships later
    // public Author Author { get; private set; } 

    // private readonly List<Comment> _comments = new();
    // public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

    public Post(string title, string content, PostType type, Guid authorId, Guid? categoryId = null)
    {
        Title = title;
        Content = content;
        Type = type;
        AuthorId = authorId;
        CategoryId = categoryId;
        IsPublished = false;
        IsFeatured = false;
        ViewCount = 0;
        LikeCount = 0;
        CommentCount = 0;
    }

    private Post() { }

    public void UpdateContent(string newContent)
    {
        Content = newContent;
        Audit(UpdatedBy ?? "System");
    }

    public void UpdateArabicContent(string? titleAr, string? contentAr)
    {
        TitleAr = titleAr;
        ContentAr = contentAr;
        Audit(UpdatedBy ?? "System");
    }

    public void Publish()
    {
        IsPublished = true;
        Audit(UpdatedBy ?? "System");
    }

    public void Unpublish()
    {
        IsPublished = false;
        Audit(UpdatedBy ?? "System");
    }

    public void SetFeatured(bool featured)
    {
        IsFeatured = featured;
        Audit(UpdatedBy ?? "System");
    }

    public void IncrementViewCount()
    {
        ViewCount++;
        Audit(UpdatedBy ?? "System");
    }

    public void IncrementLikeCount()
    {
        LikeCount++;
        Audit(UpdatedBy ?? "System");
    }

    public void DecrementLikeCount()
    {
        if (LikeCount > 0)
            LikeCount--;
        Audit(UpdatedBy ?? "System");
    }

    public void IncrementCommentCount()
    {
        CommentCount++;
        Audit(UpdatedBy ?? "System");
    }

    public void DecrementCommentCount()
    {
        if (CommentCount > 0)
            CommentCount--;
        Audit(UpdatedBy ?? "System");
    }

    public void AddTag(Tag tag)
    {
        if (!_tags.Contains(tag))
        {
            _tags.Add(tag);
            Audit(UpdatedBy ?? "System");
        }
    }

    public void RemoveTag(Tag tag)
    {
        if (_tags.Contains(tag))
        {
            _tags.Remove(tag);
            Audit(UpdatedBy ?? "System");
        }
    }

    // public void AddComment(Comment comment)
    // {
    //     _comments.Add(comment);
    // }
}
