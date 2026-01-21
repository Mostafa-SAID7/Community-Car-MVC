using System;
using System.Collections.Generic;
using CommunityCar.Domain.Base;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Domain.Entities.Community.News;

public class NewsItem : AggregateRoot
{
    public string Headline { get; private set; }
    public string Body { get; private set; }
    public string? Summary { get; private set; }
    public string? ImageUrl { get; private set; }
    public Guid AuthorId { get; private set; }
    public DateTime PublishedAt { get; private set; }
    public bool IsPublished { get; private set; }
    public bool IsFeatured { get; private set; }
    public bool IsPinned { get; private set; }
    
    // Engagement metrics
    public int ViewCount { get; private set; }
    public int LikeCount { get; private set; }
    public int CommentCount { get; private set; }
    public int ShareCount { get; private set; }
    
    // Content categorization
    public NewsCategory Category { get; private set; }
    public string? Source { get; private set; }
    public string? SourceUrl { get; private set; }
    
    // SEO and metadata
    public string? MetaTitle { get; private set; }
    public string? MetaDescription { get; private set; }
    public string? Slug { get; private set; }
    
    // Tags and topics
    private readonly List<string> _tags = new();
    public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();
    
    // Additional media
    private readonly List<string> _imageUrls = new();
    public IReadOnlyCollection<string> ImageUrls => _imageUrls.AsReadOnly();
    
    // Automotive specific
    public string? CarMake { get; private set; }
    public string? CarModel { get; private set; }
    public int? CarYear { get; private set; }

    public NewsItem(string headline, string body, Guid authorId, NewsCategory category = NewsCategory.General)
    {
        Headline = headline;
        Body = body;
        AuthorId = authorId;
        Category = category;
        PublishedAt = DateTime.UtcNow;
        IsPublished = false;
        IsFeatured = false;
        IsPinned = false;
        ViewCount = 0;
        LikeCount = 0;
        CommentCount = 0;
        ShareCount = 0;
        Slug = GenerateSlug(headline);
    }

    private NewsItem() { }

    public void UpdateContent(string headline, string body, string? summary = null)
    {
        Headline = headline;
        Body = body;
        Summary = summary;
        Slug = GenerateSlug(headline);
        Audit(UpdatedBy);
    }

    public void SetMainImage(string imageUrl)
    {
        ImageUrl = imageUrl;
        Audit(UpdatedBy);
    }

    public void AddImage(string imageUrl)
    {
        if (!_imageUrls.Contains(imageUrl))
        {
            _imageUrls.Add(imageUrl);
            Audit(UpdatedBy);
        }
    }

    public void RemoveImage(string imageUrl)
    {
        if (_imageUrls.Remove(imageUrl))
        {
            Audit(UpdatedBy);
        }
    }

    public void Publish()
    {
        IsPublished = true;
        PublishedAt = DateTime.UtcNow;
        Audit(UpdatedBy);
    }

    public void Unpublish()
    {
        IsPublished = false;
        Audit(UpdatedBy);
    }

    public void SetFeatured(bool featured)
    {
        IsFeatured = featured;
        Audit(UpdatedBy);
    }

    public void SetPinned(bool pinned)
    {
        IsPinned = pinned;
        Audit(UpdatedBy);
    }

    public void UpdateCategory(NewsCategory category)
    {
        Category = category;
        Audit(UpdatedBy);
    }

    public void SetSource(string? source, string? sourceUrl = null)
    {
        Source = source;
        SourceUrl = sourceUrl;
        Audit(UpdatedBy);
    }

    public void UpdateSeoData(string? metaTitle, string? metaDescription)
    {
        MetaTitle = metaTitle;
        MetaDescription = metaDescription;
        Audit(UpdatedBy);
    }

    public void AddTag(string tag)
    {
        if (!_tags.Contains(tag.ToLowerInvariant()))
        {
            _tags.Add(tag.ToLowerInvariant());
            Audit(UpdatedBy);
        }
    }

    public void RemoveTag(string tag)
    {
        if (_tags.Remove(tag.ToLowerInvariant()))
        {
            Audit(UpdatedBy);
        }
    }

    public void SetCarInfo(string? carMake, string? carModel, int? carYear)
    {
        CarMake = carMake;
        CarModel = carModel;
        CarYear = carYear;
        Audit(UpdatedBy);
    }

    public void IncrementViewCount()
    {
        ViewCount++;
    }

    public void IncrementLikeCount()
    {
        LikeCount++;
    }

    public void DecrementLikeCount()
    {
        if (LikeCount > 0)
            LikeCount--;
    }

    public void IncrementCommentCount()
    {
        CommentCount++;
    }

    public void DecrementCommentCount()
    {
        if (CommentCount > 0)
            CommentCount--;
    }

    public void IncrementShareCount()
    {
        ShareCount++;
    }

    private static string GenerateSlug(string title)
    {
        return title.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("'", "")
            .Replace("\"", "")
            .Replace(".", "")
            .Replace(",", "")
            .Replace("!", "")
            .Replace("?", "")
            .Replace(":", "")
            .Replace(";", "")
            .Trim('-');
    }

    public string CarDisplayName => 
        !string.IsNullOrEmpty(CarMake) && !string.IsNullOrEmpty(CarModel) 
            ? $"{CarYear} {CarMake} {CarModel}".Trim()
            : !string.IsNullOrEmpty(CarMake) 
                ? CarMake 
                : string.Empty;
}
