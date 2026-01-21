using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Domain.Entities.Localization;

public class LocalizationResource
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(200)]
    public string Key { get; set; } = string.Empty; // e.g., "QA", "HeaderTitle"

    [Required]
    public string Value { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Culture { get; set; } = string.Empty; // e.g., "en-US", "ar-EG"

    [MaxLength(200)]
    public string? ResourceGroup { get; set; } // e.g., "SharedResource", "Views.Community.QA.Index"
}
