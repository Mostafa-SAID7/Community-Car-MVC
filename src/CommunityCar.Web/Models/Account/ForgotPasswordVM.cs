using System.ComponentModel.DataAnnotations;
using CommunityCar.Web;

namespace CommunityCar.Web.Models.Account;

public class ForgotPasswordVM
{
    [Required(ErrorMessageResourceName = "EmailRequired", ErrorMessageResourceType = typeof(AccountResource))]
    [EmailAddress(ErrorMessageResourceName = "InvalidEmailFormat", ErrorMessageResourceType = typeof(AccountResource))]
    [Display(Name = "Email", ResourceType = typeof(AccountResource))]
    public string Email { get; set; } = string.Empty;
}