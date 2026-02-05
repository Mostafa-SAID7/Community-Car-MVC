using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Areas.Identity.ViewModels.Media;

/// <summary>
/// ViewModel for adding images to gallery
/// </summary>
public class AddGalleryImageVM
{
    public Guid CollectionId { get; set; }

    [Required]
    public string ImageUrl { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Title { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    public string? Tags { get; set; }
    public int SortOrder { get; set; }
}
