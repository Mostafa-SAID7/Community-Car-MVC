# Model Reorganization Summary

## Overview
This document outlines the reorganization of models across Account, Authentication, and Profile domains to eliminate duplicates and improve separation of concerns.

## Status: ✅ COMPLETED
All compilation errors have been resolved and the solution builds successfully.

## Application Layer Models

### Account Models (`CommunityCar.Application.Common.Models.Account`)
- **Purpose**: Account management operations (deactivation, deletion, data export)
- **Files**: `AccountModels.cs`
- **Key Models**:
  - `DeactivateAccountRequest`
  - `DeleteAccountRequest` 
  - `ExportUserDataRequest`
  - `AccountInfoVM`
  - `DataExportVM`
  - `ConsentVM`

### Authentication Models (`CommunityCar.Application.Common.Models.Authentication`)
- **Purpose**: Authentication operations (login, registration, password reset)
- **Files**: `AuthenticationRequests.cs`
- **Key Models**:
  - `RegisterRequest`
  - `LoginRequest`
  - `ResetPasswordRequest`
  - `ChangePasswordRequest`
  - `ForgotPasswordRequest`
  - `ConfirmEmailRequest`

### Profile Models (`CommunityCar.Application.Common.Models.Profile`)
- **Purpose**: User profile and settings management
- **Files**: `ProfileModels.cs`
- **Key Models**:
  - `UpdatePrivacySettingsRequest`
  - `UpdateNotificationSettingsRequest`
  - `ProfileSettingsVM`
  - `PrivacySettingsVM`
  - `NotificationSettingsVM`

### Security Models (`CommunityCar.Application.Common.Models.Security`)
- **Purpose**: Security-related operations (2FA, sessions, logs)
- **Files**: `SecurityModels.cs`
- **Key Models**:
  - `TwoFactorSetupRequest`
  - `ActiveSessionVM`
  - `SecurityLogVM`
  - `SecurityInfoVM`
  - `TwoFactorSetupVM`

## Web Layer ViewModels

### Account ViewModels (`CommunityCar.Web.Models.Account`)
- **Management**: Account lifecycle operations
  - `AccountInfoVM` - Account overview
  - `DataExportVM` - Data export requests
  - `DeactivateAccountVM` - Account deactivation
  - `DeleteAccountVM` - Account deletion
- **External**: OAuth/external account linking
  - `LinkedAccountVM` - Linked external accounts

### Authentication ViewModels (`CommunityCar.Web.Models.Auth`)
- **Login**: Login-related models
  - `LoginVM` - Basic login
  - `ExternalLoginVM` - OAuth login
  - `ExternalLoginConfirmationVM` - OAuth confirmation
- **Registration**: User registration
  - `RegisterVM` - User registration
- **Password Reset**: Password recovery
  - `ForgotPasswordVM` - Password reset request
  - `ResetPasswordVM` - Password reset form
- **OAuth**: Social login providers
  - `FacebookSignInVM`
  - `GoogleSignInVM`

### Profile ViewModels (`CommunityCar.Web.Models.Profile`)
- **Core**: Basic profile information
  - `ProfileSettingsVM` - Profile editing
  - `UpdateProfileVM` - Profile updates
  - `ProfileIndexVM` - Profile display
- **Following**: Social connections
  - `FollowingVM` - Following relationships
  - `FollowStatsVM` - Following statistics
  - `UserFollowListVM` - Follow lists
- **Security**: Security settings
  - `SecurityVM` - Security overview
  - `SecurityOverviewVM` - Comprehensive security status
  - `ChangePasswordVM` - Password changes
  - `TwoFactorVM` - 2FA management
  - `SecurityLogItemVM` - Security logs
- **Settings**: User preferences
  - `NotificationSettingsVM` - Notification preferences
  - `PrivacySettingsVM` - Privacy settings

## Changes Made

### Removed Duplicates
- Consolidated duplicate `TwoFactorVM` models (kept Profile version)
- Consolidated duplicate `ExternalLoginDisplayVM` models (kept Auth version)
- Removed duplicate `EnableTwoFactorVM` from Account folder

### Moved Models
- Security-related models moved from Account to Security namespace
- Profile-specific settings separated from Account management
- Authentication models properly separated from Account models

### Improved Organization
- Clear separation of concerns between domains
- Consistent naming conventions
- Proper namespace organization with re-export files
- Updated global usings for easier access

### Fixed Compilation Errors
- Added missing `Reactivate()` and `Deactivate()` methods to `UserFollowing` entity
- Fixed property references in `FollowController` (`FollowerUserId` → `FollowerId`, `UserId` → `FollowedUserId`)
- Fixed method signature usage for `GetMutualFollowersAsync`

## Benefits
1. **Eliminated Duplicates**: No more conflicting model definitions
2. **Clear Separation**: Each domain has its own models
3. **Better Maintainability**: Easier to find and update models
4. **Consistent Structure**: Uniform organization across layers
5. **Backward Compatibility**: Global usings maintain existing imports
6. **Zero Compilation Errors**: All layers build successfully