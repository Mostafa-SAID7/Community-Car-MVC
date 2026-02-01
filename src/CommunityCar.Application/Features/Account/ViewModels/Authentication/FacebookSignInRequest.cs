using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Account.ViewModels.Authentication;

public class FacebookSignInRequest
{
    public string AccessToken { get; set; } = string.Empty;
}