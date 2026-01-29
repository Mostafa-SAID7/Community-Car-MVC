using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Account.ViewModels.Authentication;

#region Authentication Models

public class RegisterVM
{
    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, ErrorMessage = "Password must be at least {2} characters long", MinimumLength = 8)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please confirm your password")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare("Password", ErrorMessage = "Password and confirmation password do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "You must accept the terms and conditions")]
    [Display(Name = "I accept the Terms and Conditions")]
    public bool AcceptTerms { get; set; }

    [Display(Name = "I want to receive marketing emails")]
    public bool AcceptMarketing { get; set; }
}

public class LoginVM
{
    [Required(ErrorMessage = "Email, username, or phone number is required")]
    [Display(Name = "Email, Username, or Phone")]
    public string LoginIdentifier { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}

public class ResetPasswordVM
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string? UserId { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 8)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Compare("Password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;
}

public class ForgotPasswordVM
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}

public class ChangePasswordVM
{
    public Guid UserId { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 8)]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Compare("NewPassword")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

#endregion

#region OAuth Models

public class OAuthConnectionsVM
{
    public bool HasGoogleAccount { get; set; }
    public bool HasFacebookAccount { get; set; }
    public DateTime? GoogleLinkedAt { get; set; }
    public DateTime? FacebookLinkedAt { get; set; }
    public List<string> AvailableProviders { get; set; } = new();
}

public class GoogleSignInRequest
{
    public string IdToken { get; set; } = string.Empty;
}

public class FacebookSignInRequest
{
    public string AccessToken { get; set; } = string.Empty;
}

public class LinkExternalAccountRequest
{
    public string UserId { get; set; } = string.Empty;
    public string ExternalToken { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string? ExternalId { get; set; }
    public string? Email { get; set; }
}

public class UnlinkExternalAccountRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
}

public class ExternalUserInfo
{
    public string Provider { get; set; } = string.Empty;
    public string ExternalId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
}

public class GoogleUserInfo : ExternalUserInfo { }
public class FacebookUserInfo : ExternalUserInfo { }

#endregion

#region Token & Requests

public class CreateTokenRequest
{
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
}

#endregion

#region Aliases

public class RegisterRequest : RegisterVM { }
public class LoginRequest : LoginVM { }
public class ResetPasswordRequest : ResetPasswordVM { }
public class ForgotPasswordRequest : ForgotPasswordVM { }
public class ChangePasswordRequest : ChangePasswordVM { }
public class GoogleSignInVM : GoogleSignInRequest { }
public class FacebookSignInVM : FacebookSignInRequest { }

#endregion
