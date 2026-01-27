using CommunityCar.Domain.Entities.Profile;

namespace CommunityCar.Application.Features.Profile.DTOs;

public class UserGalleryItemDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string MediaUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public MediaType MediaType { get; set; }
    public List<string> Tags { get; set; } = new();
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public bool IsPublic { get; set; }
    public bool IsFeatured { get; set; }
    public DateTime UploadedAt { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
}

public class CreateGalleryItemRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public MediaType MediaType { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool IsPublic { get; set; } = true;
}

public class UpdateGalleryItemRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string> Tags { get; set; } = new();
}

public class GalleryFilterRequest
{
    public MediaType? MediaType { get; set; }
    public bool? IsPublic { get; set; }
    public bool? IsFeatured { get; set; }
    public string? Tag { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}


