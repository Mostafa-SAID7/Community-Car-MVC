using System.ComponentModel.DataAnnotations;
using CommunityCar.Web;

namespace CommunityCar.Web.Models.Account;

public class ResetPasswordVM
{
    [Required(ErrorMessageResourceName = "EmailRequired", ErrorMessageResourceType = typeof(AccountResource))]
    [EmailAddress(ErrorMessageResourceName = "InvalidEmailFormat", ErrorMessageResourceType = typeof(AccountResource))]
    [Display(Name = "Email", ResourceType = typeof(AccountResource))]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessageResourceName = "PasswordRequired", ErrorMessageResourceType = typeof(AccountResource))]
    [StringLength(100, ErrorMessageResourceName = "LengthRequirement", ErrorMessageResourceType = typeof(AccountResource), MinimumLength = 8)]
    [DataType(DataType.Password)]
    [Display(Name = "Password", ResourceType = typeof(AccountResource))]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
        ErrorMessageResourceName = "PasswordFormat", ErrorMessageResourceType = typeof(AccountResource))]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "ConfirmPassword", ResourceType = typeof(AccountResource))]
    [Compare("Password", ErrorMessageResourceName = "PasswordsDoNotMatch", ErrorMessageResourceType = typeof(AccountResource))]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;
}