using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Areas.Identity.ViewModels.Authentication;

public class FacebookSignInRequest
{
    public string AccessToken { get; set; } = string.Empty;
}
