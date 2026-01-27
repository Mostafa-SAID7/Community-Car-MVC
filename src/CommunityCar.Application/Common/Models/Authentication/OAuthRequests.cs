using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Common.Models.Authentication;

public class GoogleSignInRequest
{
    [Required(ErrorMessage = "Google ID token is required")]
    public string IdToken { get; set; } = string.Empty;
}

public class FacebookSignInRequest
{
    [Required(ErrorMessage = "Facebook access token is required")]
    public string AccessToken { get; set; } = string.Empty;
}

public class LinkExternalAccountRequest
{
    [Required(ErrorMessage = "User ID is required")]
    public string UserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Provider is required")]
    [RegularExpression(@"^(Google|Facebook)$", ErrorMessage = "Provider must be either 'Google' or 'Facebook'")]
    public string Provider { get; set; } = string.Empty;

    [Required(ErrorMessage = "External token is required")]
    public string ExternalToken { get; set; } = string.Empty;
}

public class UnlinkExternalAccountRequest
{
    [Required(ErrorMessage = "User ID is required")]
    public string UserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Provider is required")]
    [RegularExpression(@"^(Google|Facebook)$", ErrorMessage = "Provider must be either 'Google' or 'Facebook'")]
    public string Provider { get; set; } = string.Empty;
}


