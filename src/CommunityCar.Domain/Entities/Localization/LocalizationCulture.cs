using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Domain.Entities.Localization;

public class LocalizationCulture
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(20)]
    public string Name { get; set; } = string.Empty; // e.g., "en-US", "ar-EG"

    [Required]
    [MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty; // e.g., "English (US)", "العربية (مصر)"

    public bool IsRTL { get; set; } = false;

    public bool IsActive { get; set; } = true;

    public bool IsDefault { get; set; } = false;
}
