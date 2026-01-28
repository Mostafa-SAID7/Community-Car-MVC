using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Models.Profile.Security.TwoFactor;

public class DisableTwoFactorVM
{
    [Required(ErrorMessage = "Password is required to disable two-factor authentication")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;
}