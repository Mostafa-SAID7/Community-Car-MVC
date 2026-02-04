using System.ComponentModel.DataAnnotations;
using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Localization;

public class LocalizationCulture : BaseEntity
{
    [Required]
    [MaxLength(20)]
    public string Name { get; set; } = string.Empty; // e.g., "en-US", "ar-EG"

    [Required]
    [MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty; // e.g., "English (US)", "العربية (مصر)"

    public bool IsRTL { get; set; } = false;

    public bool IsEnabled { get; set; } = true;

    public bool IsDefault { get; set; } = false;
}
