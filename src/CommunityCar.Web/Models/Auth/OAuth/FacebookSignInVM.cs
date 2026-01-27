using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Models.Auth.OAuth;

public class FacebookSignInVM
{
    [Required]
    public string AccessToken { get; set; } = string.Empty;
    
    public string? ReturnUrl { get; set; }
}


