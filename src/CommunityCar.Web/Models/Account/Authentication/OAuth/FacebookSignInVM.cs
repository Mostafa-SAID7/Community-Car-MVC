using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Models.Account.Authentication.OAuth;

public class FacebookSignInVM
{
    [Required]
    public string AccessToken { get; set; } = string.Empty;
    
    public string? ReturnUrl { get; set; }
}


