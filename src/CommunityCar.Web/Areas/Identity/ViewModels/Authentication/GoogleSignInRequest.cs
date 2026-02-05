using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Areas.Identity.ViewModels.Authentication;

public class GoogleSignInRequest
{
    public string IdToken { get; set; } = string.Empty;
}
