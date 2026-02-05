using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Areas.Identity.ViewModels.Media;

/// <summary>
/// ViewModel for updating gallery collections
/// </summary>
public class UpdateGalleryCollectionVM
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public bool IsPublic { get; set; } = true;
    public bool AllowComments { get; set; } = true;
    public string? Tags { get; set; }
}
