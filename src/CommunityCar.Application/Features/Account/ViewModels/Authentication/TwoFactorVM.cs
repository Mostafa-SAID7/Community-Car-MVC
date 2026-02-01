using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Account.ViewModels.Authentication;

public class TwoFactorVM
{
    public bool IsEnabled { get; set; }
    public bool IsMachineRemembered { get; set; }
    public int RecoveryCodesLeft { get; set; }

    [Display(Name = "Authenticator Key")]
    public string? AuthenticatorKey { get; set; }

    [Display(Name = "Authenticator URI")]
    public string? AuthenticatorUri { get; set; }
}