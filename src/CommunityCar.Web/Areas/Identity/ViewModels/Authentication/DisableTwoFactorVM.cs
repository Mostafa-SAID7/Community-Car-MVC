using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Areas.Identity.ViewModels.Authentication;

public class DisableTwoFactorVM
{
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
