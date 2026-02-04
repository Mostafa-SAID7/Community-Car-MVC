using System.ComponentModel.DataAnnotations;
using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Features.Account.ViewModels.Media;

public class CreateGalleryCollectionVM
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
    
    public bool IsPublic { get; set; } = true;
    public bool IsFeatured { get; set; } = false;
    public List<string> Tags { get; set; } = new();
    public string? CoverImageUrl { get; set; }
}

public class GalleryStatsVM
{
    public Guid UserId { get; set; }
    public int TotalItems { get; set; }
    public int TotalCollections { get; set; }
    public int PublicItems { get; set; }
    public int PrivateItems { get; set; }
    public int FeaturedItems { get; set; }
    public long TotalStorageUsed { get; set; }
    public long StorageLimit { get; set; }
    public int TotalViews { get; set; }
    public int TotalLikes { get; set; }
    public int TotalComments { get; set; }
    public DateTime LastUpload { get; set; }
    public List<ChartDataVM> UploadTrend { get; set; } = new();
    public Dictionary<string, int> ItemsByType { get; set; } = new();
}