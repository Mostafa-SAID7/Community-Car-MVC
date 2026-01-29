using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Models.Account.Core;

public class UpdateProfileVM
{
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

    [Phone]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    [Url(ErrorMessage = "Please enter a valid URL")]
    [StringLength(200, ErrorMessage = "Website URL cannot exceed 200 characters")]
    [Display(Name = "Website")]
    public string? Website { get; set; }

    public string? ProfilePictureUrl { get; set; }
    public string? CoverImageUrl { get; set; }
}