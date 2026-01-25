using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Profile;

public class UserGallery : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public string MediaUrl { get; private set; }
    public string? ThumbnailUrl { get; private set; }
    public MediaType MediaType { get; private set; }
    public string? Tags { get; private set; } // JSON array of tags
    public int ViewCount { get; private set; }
    public int LikeCount { get; private set; }
    public bool IsPublic { get; private set; }
    public bool IsFeatured { get; private set; }
    public DateTime UploadedAt { get; private set; }

    public UserGallery(Guid userId, string title, string mediaUrl, MediaType mediaType, bool isPublic = true)
    {
        UserId = userId;
        Title = title;
        MediaUrl = mediaUrl;
        MediaType = mediaType;
        IsPublic = isPublic;
        UploadedAt = DateTime.UtcNow;
        ViewCount = 0;
        LikeCount = 0;
        IsFeatured = false;
    }

    // EF Core constructor
    private UserGallery() { }

    public void UpdateDetails(string title, string? description, string? tags)
    {
        Title = title;
        Description = description;
        Tags = tags;
        Audit(UpdatedBy);
    }

    public void SetThumbnail(string thumbnailUrl)
    {
        ThumbnailUrl = thumbnailUrl;
        Audit(UpdatedBy);
    }

    public void IncrementViews()
    {
        ViewCount++;
        Audit(UpdatedBy);
    }

    public void IncrementLikes()
    {
        LikeCount++;
        Audit(UpdatedBy);
    }

    public void DecrementLikes()
    {
        if (LikeCount > 0)
            LikeCount--;
        Audit(UpdatedBy);
    }

    public void ToggleFeatured()
    {
        IsFeatured = !IsFeatured;
        Audit(UpdatedBy);
    }

    public void ToggleVisibility()
    {
        IsPublic = !IsPublic;
        Audit(UpdatedBy);
    }
}

public enum MediaType
{
    Image = 1,
    Video = 2,
    Audio = 3
}