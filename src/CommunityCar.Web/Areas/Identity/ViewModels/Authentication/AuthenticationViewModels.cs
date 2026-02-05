using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Areas.Identity.ViewModels.Authentication;

/*
 * AUTHENTICATION VIEWMODELS REFERENCE FILE
 * ========================================
 * 
 * This file originally contained 13 ViewModels that have been split into separate files for better organization.
 * All ViewModels are now located in individual files within this same directory.
 * 
 * SPLIT FILES:
 * ============
 * 
 * Authentication ViewModels:
 * - RegisterVM.cs
 * - LoginVM.cs
 * - ResetPasswordVM.cs
 * - ForgotPasswordVM.cs
 * - ChangePasswordVM.cs
 * 
 * OAuth ViewModels:
 * - OAuthConnectionsVM.cs
 * - GoogleSignInRequest.cs
 * - FacebookSignInRequest.cs
 * - LinkExternalAccountRequest.cs
 * - UnlinkExternalAccountRequest.cs
 * - ExternalUserInfo.cs (includes GoogleUserInfo and FacebookUserInfo)
 * 
 * Token & Request ViewModels:
 * - CreateTokenRequest.cs
 * - CreateSessionRequest.cs
 * 
 * ALIASES:
 * ========
 * The following aliases are maintained for backward compatibility:
 * - RegisterRequest : RegisterVM
 * - LoginRequest : LoginVM
 * - ResetPasswordRequest : ResetPasswordVM
 * - ForgotPasswordRequest : ForgotPasswordVM
 * - ChangePasswordRequest : ChangePasswordVM
 * - GoogleSignInVM : GoogleSignInRequest
 * - FacebookSignInVM : FacebookSignInRequest
 * 
 * USAGE:
 * ======
 * Import individual ViewModels as needed:
 * using CommunityCar.Application.Features.Account.ViewModels.Authentication;
 * 
 * All ViewModels maintain the same namespace and functionality as before.
 * This split improves code organization, maintainability, and reduces file size.
 */

#region Aliases for backward compatibility

public class RegisterRequest : RegisterVM { }
public class LoginRequest : LoginVM { }
public class ResetPasswordRequest : ResetPasswordVM { }
public class ForgotPasswordRequest : ForgotPasswordVM { }
public class ChangePasswordRequest : ChangePasswordVM { }
public class GoogleSignInVM : GoogleSignInRequest { }
public class FacebookSignInVM : FacebookSignInRequest { }

#endregion

