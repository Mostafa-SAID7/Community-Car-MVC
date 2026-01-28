using System;
using System.Collections.Generic;
using CommunityCar.Domain.Base;
using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Domain.Entities.Community.Guides;

public class Guide : AggregateRoot
{
    // Basic Information
    public string Title { get; private set; }
    public string Content { get; private set; } // Markdown or HTML content
    public string? Summary { get; private set; }
    public Guid AuthorId { get; private set; }
    
    // Arabic Localization
    public string? TitleAr { get; private set; }
    public string? ContentAr { get; private set; }
    public string? SummaryAr { get; private set; }
    
    // Publication Status
    public bool IsPublished { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    
    // Verification and Quality
    public bool IsVerified { get; private set; }
    public bool IsFeatured { get; private set; }
    
    // Categorization
    public string? Category { get; private set; }
    public GuideDifficulty Difficulty { get; private set; }
    public int EstimatedMinutes { get; private set; }
    
    // Media
    public string? ThumbnailUrl { get; private set; }
    public string? CoverImageUrl { get; private set; }
    
    // Engagement Statistics
    public int ViewCount { get; private set; }
    public int BookmarkCount { get; private set; }
    public double AverageRating { get; private set; }
    public int RatingCount { get; private set; }
    
    // Tags for discoverability
    private readonly List<string> _tags = new();
    public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();
    
    // Prerequisites and Requirements
    private readonly List<string> _prerequisites = new();
    public IReadOnlyCollection<string> Prerequisites => _prerequisites.AsReadOnly();
    
    private readonly List<string> _requiredTools = new();
    public IReadOnlyCollection<string> RequiredTools => _requiredTools.AsReadOnly();

    public Guide(
        string title, 
        string content, 
        Guid authorId,
        string? summary = null,
        string? category = null,
        GuideDifficulty difficulty = GuideDifficulty.Beginner,
        int estimatedMinutes = 30,
        string? titleAr = null,
        string? contentAr = null,
        string? summaryAr = null)
    {
        Title = title;
        Content = content;
        AuthorId = authorId;
        Summary = summary;
        Category = category;
        Difficulty = difficulty;
        EstimatedMinutes = estimatedMinutes;
        TitleAr = titleAr;
        ContentAr = contentAr;
        SummaryAr = summaryAr;
        IsPublished = false;
        IsVerified = false;
        IsFeatured = false;
        ViewCount = 0;
        BookmarkCount = 0;
        AverageRating = 0;
        RatingCount = 0;
    }

    private Guide() { }

    // Publishing Methods
    public void Publish()
    {
        IsPublished = true;
        PublishedAt = DateTime.UtcNow;
        Audit(UpdatedBy ?? "System");
    }

    public void Unpublish()
    {
        IsPublished = false;
        Audit(UpdatedBy ?? "System");
    }

    // Verification and Quality
    public void Verify()
    {
        IsVerified = true;
        Audit(UpdatedBy ?? "System");
    }

    public void Feature()
    {
        IsFeatured = true;
        IsVerified = true; // Featured guides are always verified
        Audit(UpdatedBy ?? "System");
    }

    public void Unfeature()
    {
        IsFeatured = false;
        Audit(UpdatedBy ?? "System");
    }

    // Content Management
    public void UpdateBasicInfo(string title, string content, string? summary = null)
    {
        Title = title;
        Content = content;
        Summary = summary;
        Audit(UpdatedBy ?? "System");
    }

    public void UpdateArabicContent(string? titleAr, string? contentAr, string? summaryAr)
    {
        TitleAr = titleAr;
        ContentAr = contentAr;
        SummaryAr = summaryAr;
        Audit(UpdatedBy ?? "System");
    }

    public void UpdateCategory(string? category)
    {
        Category = category;
        Audit(UpdatedBy ?? "System");
    }

    public void UpdateDifficulty(GuideDifficulty difficulty)
    {
        Difficulty = difficulty;
        Audit(UpdatedBy ?? "System");
    }

    public void UpdateEstimatedTime(int minutes)
    {
        EstimatedMinutes = minutes;
        Audit(UpdatedBy ?? "System");
    }

    // Media Management
    public void UpdateThumbnail(string? thumbnailUrl)
    {
        ThumbnailUrl = thumbnailUrl;
        Audit(UpdatedBy ?? "System");
    }

    public void UpdateCoverImage(string? coverImageUrl)
    {
        CoverImageUrl = coverImageUrl;
        Audit(UpdatedBy ?? "System");
    }

    // Engagement Tracking
    public void IncrementViewCount()
    {
        ViewCount++;
    }

    public void IncrementBookmarkCount()
    {
        BookmarkCount++;
    }

    public void DecrementBookmarkCount()
    {
        if (BookmarkCount > 0)
            BookmarkCount--;
    }

    public void AddRating(double rating)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5");

        var totalRating = (AverageRating * RatingCount) + rating;
        RatingCount++;
        AverageRating = totalRating / RatingCount;
        Audit(UpdatedBy ?? "System");
    }

    // Tags Management
    public void AddTag(string tag)
    {
        if (!_tags.Contains(tag))
        {
            _tags.Add(tag);
            Audit(UpdatedBy ?? "System");
        }
    }

    public void RemoveTag(string tag)
    {
        if (_tags.Remove(tag))
        {
            Audit(UpdatedBy ?? "System");
        }
    }

    // Prerequisites Management
    public void AddPrerequisite(string prerequisite)
    {
        if (!_prerequisites.Contains(prerequisite))
        {
            _prerequisites.Add(prerequisite);
            Audit(UpdatedBy ?? "System");
        }
    }

    public void RemovePrerequisite(string prerequisite)
    {
        if (_prerequisites.Remove(prerequisite))
        {
            Audit(UpdatedBy ?? "System");
        }
    }

    // Required Tools Management
    public void AddRequiredTool(string tool)
    {
        if (!_requiredTools.Contains(tool))
        {
            _requiredTools.Add(tool);
            Audit(UpdatedBy ?? "System");
        }
    }

    public void RemoveRequiredTool(string tool)
    {
        if (_requiredTools.Remove(tool))
        {
            Audit(UpdatedBy ?? "System");
        }
    }
}
