using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Models.Account;

public class ForgotPasswordVM
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;
}