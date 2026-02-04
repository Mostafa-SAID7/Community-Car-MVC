using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Account.ViewModels.Media;

public class GalleryItemVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public DateTime UploadedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsPublic { get; set; }
    public bool IsFeatured { get; set; }
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public List<string> Tags { get; set; } = new();
    public Guid? CollectionId { get; set; }
    public string? CollectionName { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class CreateGalleryItemVM
{
    [Required]
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public IFormFile File { get; set; } = null!;
    
    public bool IsPublic { get; set; } = true;
    public bool IsFeatured { get; set; } = false;
    public List<string> Tags { get; set; } = new();
    public Guid? CollectionId { get; set; }
}

public class UpdateGalleryItemVM
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public bool IsFeatured { get; set; }
    public List<string> Tags { get; set; } = new();
    public Guid? CollectionId { get; set; }
}