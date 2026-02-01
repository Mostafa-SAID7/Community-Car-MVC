using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Account.ViewModels.Authentication;

public class GoogleSignInRequest
{
    public string IdToken { get; set; } = string.Empty;
}