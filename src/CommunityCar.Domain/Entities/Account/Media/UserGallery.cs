using CommunityCar.Domain.Base;
using CommunityCar.Domain.Enums.Account;

namespace CommunityCar.Domain.Entities.Account.Media;

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
    public int DisplayOrder { get; private set; }
    public string ImageUrl => MediaUrl;
    public long FileSize { get; private set; }
    public string? MimeType { get; private set; }
    public string? Metadata { get; private set; } // JSON for additional metadata

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

    public static UserGallery Create(Guid userId, string mediaUrl, string? caption = null, Dictionary<string, object>? metadata = null)
    {
        var gallery = new UserGallery(userId, caption ?? "Untitled", mediaUrl, MediaType.Image);
        if (metadata != null)
        {
            gallery.UpdateMetadata(metadata);
        }
        return gallery;
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

    public void UpdateCaption(string? caption)
    {
        Description = caption;
        Audit(UpdatedBy);
    }

    public void UpdateMetadata(Dictionary<string, object> metadata)
    {
        Metadata = System.Text.Json.JsonSerializer.Serialize(metadata);
        Audit(UpdatedBy);
    }

    public void SetDisplayOrder(int order)
    {
        DisplayOrder = order;
        Audit(UpdatedBy);
    }

    public void AddTag(string tag)
    {
        var tagList = string.IsNullOrEmpty(Tags) ? new List<string>() : System.Text.Json.JsonSerializer.Deserialize<List<string>>(Tags) ?? new List<string>();
        if (!tagList.Contains(tag))
        {
            tagList.Add(tag);
            Tags = System.Text.Json.JsonSerializer.Serialize(tagList);
            Audit(UpdatedBy);
        }
    }

    public void RemoveTag(string tag)
    {
        if (string.IsNullOrEmpty(Tags)) return;
        var tagList = System.Text.Json.JsonSerializer.Deserialize<List<string>>(Tags);
        if (tagList != null && tagList.Remove(tag))
        {
            Tags = System.Text.Json.JsonSerializer.Serialize(tagList);
            Audit(UpdatedBy);
        }
    }

    public void SetVisibility(bool isPublic)
    {
        IsPublic = isPublic;
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