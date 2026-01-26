using System.ComponentModel.DataAnnotations;
using CommunityCar.Web;

namespace CommunityCar.Web.Models.Account;

public class LoginVM
{
    [Required(ErrorMessageResourceName = "EmailRequired", ErrorMessageResourceType = typeof(AccountResource))]
    [EmailAddress(ErrorMessageResourceName = "InvalidEmailFormat", ErrorMessageResourceType = typeof(AccountResource))]
    [Display(Name = "Email", ResourceType = typeof(AccountResource))]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessageResourceName = "PasswordRequired", ErrorMessageResourceType = typeof(AccountResource))]
    [DataType(DataType.Password)]
    [Display(Name = "Password", ResourceType = typeof(AccountResource))]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "RememberMe", ResourceType = typeof(AccountResource))]
    public bool RememberMe { get; set; }
}
