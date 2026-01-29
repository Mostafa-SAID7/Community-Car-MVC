namespace CommunityCar.Application.Features.Account.DTOs.Media;

#region User Gallery DTOs

public class UserGalleryDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool IsPublic { get; set; }
    public int DisplayOrder { get; set; }
    public long FileSize { get; set; }
    public string? MimeType { get; set; }
    public int ViewCount { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class AddGalleryItemRequest
{
    public Guid UserId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool IsPublic { get; set; } = true;
    public long FileSize { get; set; }
    public string? MimeType { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}

public class UpdateGalleryItemRequest
{
    public Guid ItemId { get; set; }
    public string? Caption { get; set; }
    public List<string>? Tags { get; set; }
    public bool? IsPublic { get; set; }
    public int? DisplayOrder { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}

public class RemoveGalleryItemRequest
{
    public Guid ItemId { get; set; }
    public Guid UserId { get; set; }
}

#endregion