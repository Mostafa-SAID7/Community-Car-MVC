using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Models.Auth.OAuth;

public class GoogleSignInVM
{
    [Required]
    public string IdToken { get; set; } = string.Empty;
    
    public string? ReturnUrl { get; set; }
}


