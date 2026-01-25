using System;
using System.Collections.Generic;
using CommunityCar.Domain.Base;
using CommunityCar.Domain.Enums;
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
    
    // Simple navigation property placeholder - replace with actual relationships later
    // public Author Author { get; private set; } 

    // private readonly List<Comment> _comments = new();
    // public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

    public Post(string title, string content, PostType type, Guid authorId)
    {
        Title = title;
        Content = content;
        Type = type;
        AuthorId = authorId;
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

    // public void AddComment(Comment comment)
    // {
    //     _comments.Add(comment);
    // }
}
