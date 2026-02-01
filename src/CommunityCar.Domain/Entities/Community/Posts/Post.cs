using System;
using System.Collections.Generic;
using CommunityCar.Domain.Base;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Entities.Account.Core;
using CommunityCar.Domain.Entities.Shared;

namespace CommunityCar.Domain.Entities.Community.Posts;

public class Post : AggregateRoot
{
    public string Title { get; private set; }
    public string Content { get; private set; }
    public string Slug { get; private set; } = string.Empty;
    
    // Arabic Localization
    public string? TitleAr { get; private set; }
    public string? ContentAr { get; private set; }
    
    public PostType Type { get; private set; }
    public Guid AuthorId { get; private set; }
    public virtual User Author { get; private set; } = null!;
    public Guid? CategoryId { get; private set; }
    public Guid? GroupId { get; private set; }
    
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
        Slug = GenerateSlug(title);
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

    public void UpdateTitle(string newTitle)
    {
        Title = newTitle;
        Slug = GenerateSlug(newTitle);
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

    private static string GenerateSlug(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Guid.NewGuid().ToString("N")[..8];

        var slug = title.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("'", "")
            .Replace("\"", "")
            .Replace(".", "")
            .Replace(",", "")
            .Replace("!", "")
            .Replace("?", "")
            .Replace(":", "")
            .Replace(";", "")
            .Replace("(", "")
            .Replace(")", "")
            .Replace("[", "")
            .Replace("]", "")
            .Replace("{", "")
            .Replace("}", "")
            .Replace("/", "-")
            .Replace("\\", "-")
            .Replace("&", "and");

        // Remove multiple consecutive dashes
        while (slug.Contains("--"))
            slug = slug.Replace("--", "-");

        // Remove leading and trailing dashes
        slug = slug.Trim('-');

        // Ensure slug is not empty and not too long
        if (string.IsNullOrWhiteSpace(slug))
            slug = Guid.NewGuid().ToString("N")[..8];
        else if (slug.Length > 100)
            slug = slug[..100].TrimEnd('-');

        return slug;
    }
}
