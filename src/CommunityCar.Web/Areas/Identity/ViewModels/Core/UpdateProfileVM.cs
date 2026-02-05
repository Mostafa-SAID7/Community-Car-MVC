using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Areas.Identity.ViewModels.Core;

public class UpdateProfileVM
{
    public Guid UserId { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Bio")]
    public string? Bio { get; set; }

    [StringLength(100)]
    [Display(Name = "City")]
    public string? City { get; set; }

    [Display(Name = "Country")]
    public string? Country { get; set; }

    [StringLength(500)]
    [Display(Name = "Bio (Arabic)")]
    public string? BioAr { get; set; }

    [StringLength(100)]
    [Display(Name = "City (Arabic)")]
    public string? CityAr { get; set; }

    [StringLength(100)]
    [Display(Name = "Country (Arabic)")]
    public string? CountryAr { get; set; }

    [Url(ErrorMessage = "Please enter a valid URL")]
    public string? Website { get; set; }

    [Phone]
    public string? PhoneNumber { get; set; }
}
