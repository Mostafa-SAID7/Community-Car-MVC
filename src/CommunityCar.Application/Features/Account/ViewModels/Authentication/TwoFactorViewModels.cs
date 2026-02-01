using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Account.ViewModels.Authentication;

/// <summary>
/// Two-Factor Authentication ViewModels and Request Models
/// 
/// This file has been reorganized for better maintainability. Each ViewModel and Request model
/// is now in its own separate file within this namespace. This improves code organization,
/// reduces merge conflicts, and makes the codebase easier to navigate.
/// 
/// Individual files:
/// - TwoFactorVM.cs - Two-factor authentication status and settings
/// - TwoFactorSetupVM.cs - Setup view model with QR code and backup codes
/// - EnableTwoFactorVM.cs - View model for enabling two-factor authentication
/// - DisableTwoFactorVM.cs - View model for disabling two-factor authentication
/// - TwoFactorSetupRequest.cs - Request model for setting up two-factor authentication
/// - EnableTwoFactorRequest.cs - Request model for enabling two-factor authentication
/// - DisableTwoFactorRequest.cs - Request model for disabling two-factor authentication
/// - VerifyTwoFactorTokenRequest.cs - Request model for verifying two-factor tokens
/// - GenerateRecoveryCodesRequest.cs - Request model for generating recovery codes
/// - VerifyRecoveryCodeRequest.cs - Request model for verifying recovery codes
/// - SendSmsTokenRequest.cs - Request model for sending SMS tokens
/// - TwoFactorChallengeResult.cs - Result model for two-factor challenges
/// 
/// All classes maintain their original functionality and public API.
/// </summary>

// The individual classes are now in separate files within this namespace.
// This file is kept for documentation and backward compatibility.

// Aliases
public delegate void SuccessAction(); // placeholder if needed