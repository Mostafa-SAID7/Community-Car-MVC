using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Models.Account.Authentication.Login.External;

public class ExternalLoginConfirmationVM
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "First name is required")]
    [Display(Name = "First Name")]
    [StringLength(50, MinimumLength = 1)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [Display(Name = "Last Name")]
    [StringLength(50, MinimumLength = 1)]
    public string LastName { get; set; } = string.Empty;
}