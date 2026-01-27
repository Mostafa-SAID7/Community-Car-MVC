using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Models.Auth.PasswordReset;

public class ForgotPasswordVM
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;
}


