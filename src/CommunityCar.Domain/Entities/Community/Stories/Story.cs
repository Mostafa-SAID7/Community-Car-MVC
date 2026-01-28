using System;
using System.Collections.Generic;
using CommunityCar.Domain.Base;
using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Domain.Entities.Community.Stories;

public class Story : AggregateRoot
{
    public string MediaUrl { get; private set; }
    public Guid AuthorId { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    
    // Enhanced properties
    public string? Caption { get; private set; }
    public string? CaptionAr { get; private set; }
    public StoryType Type { get; private set; }
    public string? ThumbnailUrl { get; private set; }
    public int Duration { get; private set; } // in seconds
    
    // Engagement metrics
    public int ViewCount { get; private set; }
    public int LikeCount { get; private set; }
    public int ReplyCount { get; private set; }
    public int ShareCount { get; private set; }
    
    // Story status
    public bool IsActive { get; private set; }
    public bool IsArchived { get; private set; }
    public bool IsFeatured { get; private set; }
    public bool IsHighlighted { get; private set; }
    
    // Location and context
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }
    public string? LocationName { get; private set; }
    
    // Automotive context
    public string? CarMake { get; private set; }
    public string? CarModel { get; private set; }
    public int? CarYear { get; private set; }
    public string? EventType { get; private set; } // Road trip, Car show, Maintenance, etc.
    
    // Privacy and visibility
    public StoryVisibility Visibility { get; private set; }
    public bool AllowReplies { get; private set; }
    public bool AllowSharing { get; private set; }
    
    // Tags and mentions
    private readonly List<string> _tags = new();
    private readonly List<Guid> _mentionedUsers = new();
    public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();
    public IReadOnlyCollection<Guid> MentionedUsers => _mentionedUsers.AsReadOnly();
    
    // Additional media for multi-part stories
    private readonly List<string> _additionalMediaUrls = new();
    public IReadOnlyCollection<string> AdditionalMediaUrls => _additionalMediaUrls.AsReadOnly();

    public Story(string mediaUrl, Guid authorId, StoryType type = StoryType.Image, int duration = 24)
    {
        MediaUrl = mediaUrl;
        AuthorId = authorId;
        Type = type;
        Duration = duration;
        ExpiresAt = DateTime.UtcNow.AddHours(duration);
        IsActive = true;
        IsArchived = false;
        IsFeatured = false;
        IsHighlighted = false;
        ViewCount = 0;
        LikeCount = 0;
        ReplyCount = 0;
        ShareCount = 0;
        Visibility = StoryVisibility.Public;
        AllowReplies = true;
        AllowSharing = true;
    }

    private Story() { }

    public void UpdateCaption(string? caption)
    {
        Caption = caption;
        Audit(UpdatedBy);
    }

    public void UpdateArabicContent(string? captionAr)
    {
        CaptionAr = captionAr;
        Audit(UpdatedBy);
    }

    public void SetThumbnail(string thumbnailUrl)
    {
        ThumbnailUrl = thumbnailUrl;
        Audit(UpdatedBy);
    }

    public void SetLocation(double? latitude, double? longitude, string? locationName = null)
    {
        Latitude = latitude;
        Longitude = longitude;
        LocationName = locationName;
        Audit(UpdatedBy);
    }

    public void SetCarInfo(string? carMake, string? carModel, int? carYear, string? eventType = null)
    {
        CarMake = carMake;
        CarModel = carModel;
        CarYear = carYear;
        EventType = eventType;
        Audit(UpdatedBy);
    }

    public void SetVisibility(StoryVisibility visibility)
    {
        Visibility = visibility;
        Audit(UpdatedBy);
    }

    public void SetInteractionSettings(bool allowReplies, bool allowSharing)
    {
        AllowReplies = allowReplies;
        AllowSharing = allowSharing;
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

    public void MentionUser(Guid userId)
    {
        if (!_mentionedUsers.Contains(userId))
        {
            _mentionedUsers.Add(userId);
            Audit(UpdatedBy);
        }
    }

    public void UnmentionUser(Guid userId)
    {
        if (_mentionedUsers.Remove(userId))
        {
            Audit(UpdatedBy);
        }
    }

    public void AddMedia(string mediaUrl)
    {
        if (!_additionalMediaUrls.Contains(mediaUrl))
        {
            _additionalMediaUrls.Add(mediaUrl);
            Audit(UpdatedBy);
        }
    }

    public void RemoveMedia(string mediaUrl)
    {
        if (_additionalMediaUrls.Remove(mediaUrl))
        {
            Audit(UpdatedBy);
        }
    }

    public void SetFeatured(bool featured)
    {
        IsFeatured = featured;
        Audit(UpdatedBy);
    }

    public void SetHighlighted(bool highlighted)
    {
        IsHighlighted = highlighted;
        Audit(UpdatedBy);
    }

    public void Archive()
    {
        IsArchived = true;
        IsActive = false;
        Audit(UpdatedBy);
    }

    public void Restore()
    {
        IsArchived = false;
        IsActive = !IsExpired;
        Audit(UpdatedBy);
    }

    public void ExtendDuration(int additionalHours)
    {
        ExpiresAt = ExpiresAt.AddHours(additionalHours);
        if (!IsExpired)
        {
            IsActive = true;
        }
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

    public void IncrementReplyCount()
    {
        ReplyCount++;
    }

    public void DecrementReplyCount()
    {
        if (ReplyCount > 0)
            ReplyCount--;
    }

    public void IncrementShareCount()
    {
        ShareCount++;
    }

    public bool IsExpired => DateTime.UtcNow > ExpiresAt;

    public TimeSpan TimeRemaining => IsExpired ? TimeSpan.Zero : ExpiresAt - DateTime.UtcNow;

    public string CarDisplayName => 
        !string.IsNullOrEmpty(CarMake) && !string.IsNullOrEmpty(CarModel) 
            ? $"{CarYear} {CarMake} {CarModel}".Trim()
            : !string.IsNullOrEmpty(CarMake) 
                ? CarMake 
                : string.Empty;

    public bool IsMultiMedia => _additionalMediaUrls.Any();

    public int TotalMediaCount => 1 + _additionalMediaUrls.Count;
}
