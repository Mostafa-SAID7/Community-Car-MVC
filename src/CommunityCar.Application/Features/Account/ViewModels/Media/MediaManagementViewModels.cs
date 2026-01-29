namespace CommunityCar.Application.Features.Account.ViewModels.Media;

public class UploadGalleryItemVM
{
    public Guid UserId { get; set; }
    public string? Caption { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool IsPublic { get; set; } = true;
    public IFormFile? ImageFile { get; set; }
    public string? ImageUrl { get; set; }
    public long MaxFileSize { get; set; } = 5 * 1024 * 1024; // 5MB
    public List<string> AllowedFileTypes { get; set; } = new() { "image/jpeg", "image/png", "image/gif", "image/webp" };
}

public class EditGalleryItemVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool IsPublic { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ViewCount { get; set; }
}

public class GalleryTagsVM
{
    public Guid UserId { get; set; }
    public List<TagUsageVM> Tags { get; set; } = new();
    public int TotalTags { get; set; }
}

public class TagUsageVM
{
    public string Tag { get; set; } = string.Empty;
    public int UsageCount { get; set; }
    public DateTime LastUsed { get; set; }
    public string Color { get; set; } = string.Empty;
}