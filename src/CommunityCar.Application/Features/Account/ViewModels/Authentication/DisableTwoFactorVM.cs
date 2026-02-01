using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Account.ViewModels.Authentication;

public class DisableTwoFactorVM
{
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}