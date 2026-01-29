using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Models.Account.Authentication.OAuth;

public class GoogleSignInVM
{
    [Required]
    public string IdToken { get; set; } = string.Empty;
    
    public string? ReturnUrl { get; set; }
}


