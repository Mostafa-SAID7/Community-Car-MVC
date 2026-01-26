using System.ComponentModel.DataAnnotations;
using CommunityCar.Web;

namespace CommunityCar.Web.Models.Account;

public class ChangePasswordVM
{
    [Required(ErrorMessageResourceName = "PasswordRequired", ErrorMessageResourceType = typeof(AccountResource))]
    [DataType(DataType.Password)]
    [Display(Name = "Password", ResourceType = typeof(AccountResource))]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessageResourceName = "PasswordRequired", ErrorMessageResourceType = typeof(AccountResource))]
    [StringLength(100, ErrorMessageResourceName = "LengthRequirement", ErrorMessageResourceType = typeof(AccountResource), MinimumLength = 8)]
    [DataType(DataType.Password)]
    [Display(Name = "Password", ResourceType = typeof(AccountResource))]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
        ErrorMessageResourceName = "PasswordFormat", ErrorMessageResourceType = typeof(AccountResource))]
    public string NewPassword { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "ConfirmPassword", ResourceType = typeof(AccountResource))]
    [Compare("NewPassword", ErrorMessageResourceName = "PasswordsDoNotMatch", ErrorMessageResourceType = typeof(AccountResource))]
    public string ConfirmPassword { get; set; } = string.Empty;
}