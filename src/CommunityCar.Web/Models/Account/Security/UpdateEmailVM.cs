using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Models.Account.Security;

public class UpdateEmailVM
{
    [Required]
    [EmailAddress]
    [Display(Name = "New Email")]
    public string NewEmail { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;
}