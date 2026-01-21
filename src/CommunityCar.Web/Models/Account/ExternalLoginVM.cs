using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Models.Account;

public class ExternalLoginVM
{
    public string Provider { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;
}

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

public class ManageExternalLoginsVM
{
    public IList<ExternalLoginDisplayVM> CurrentLogins { get; set; } = new List<ExternalLoginDisplayVM>();
    public IList<string> OtherLogins { get; set; } = new List<string>();
    public bool ShowRemoveButton { get; set; }
}

public class ExternalLoginDisplayVM
{
    public string LoginProvider { get; set; } = string.Empty;
    public string ProviderDisplayName { get; set; } = string.Empty;
    public string ProviderKey { get; set; } = string.Empty;
    public DateTime LinkedAt { get; set; }
}