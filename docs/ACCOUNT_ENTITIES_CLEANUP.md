# Account Entities Cleanup and Organization

## Overview
This document outlines the comprehensive cleanup and reorganization of account-related entities to eliminate duplicate properties, improve organization, and implement proper user tracking.

## Changes Made

### 1. User Entity Cleanup
- **Removed duplicate properties**: Moved profile-related properties to `UserProfile` value object
- **Removed OTP/Token properties**: Moved to dedicated `UserToken` entity
- **Added navigation properties**: Proper relationships with related entities
- **Fixed ProfileService**: Updated to use value objects instead of direct properties

### 2. New Entities Created

#### UserToken Entity
- **Purpose**: Centralized OTP and token management
- **Features**:
  - Support for multiple token types (Email verification, SMS, 2FA, etc.)
  - Automatic expiration handling
  - Attempt counting and rate limiting
  - Metadata support for additional context

#### UserSession Entity
- **Purpose**: Proper session tracking and management
- **Features**:
  - Device and location tracking
  - Session duration calculation
  - Activity tracking
  - Automatic expiration handling

#### DailyContentAnalytics Entity
- **Purpose**: Separated from UserAnalytics for better organization
- **Features**:
  - Content creation metrics
  - Engagement tracking
  - Daily aggregation

### 3. Value Objects Organization
All value objects are properly organized in `src/CommunityCar.Domain/ValueObjects/Account/`:
- `UserProfile.cs` - Profile information
- `PrivacySettings.cs` - Privacy preferences
- `NotificationSettings.cs` - Notification preferences
- `TwoFactorSettings.cs` - 2FA configuration
- `OAuthInfo.cs` - OAuth provider information

### 4. Enums Organization
All account-related enums are in `src/CommunityCar.Domain/Enums/Account/`:
- `BadgeCategory.cs`
- `BadgeRarity.cs`
- `MediaType.cs`
- `TokenType.cs` (new)

## Entity Responsibilities

### User (Main Entity)
- Identity and authentication
- Basic account information
- Relationships with other entities
- Domain events

### UserActivity
- User action tracking
- Points and gamification
- Location and device info

### UserAchievement
- Achievement progress tracking
- Reward management
- Localization support

### UserBadge
- Badge collection
- Display preferences
- Categorization

### UserFollowing
- Social relationships
- Interaction tracking
- Notification preferences

### UserGallery
- Media management
- Privacy controls
- Engagement metrics

### UserInterest
- Interest tracking
- Score calculation
- Staleness detection

### UserManagementAction
- Administrative actions
- Audit trail
- Reversibility tracking

### UserAnalytics
- Aggregated user metrics
- Daily statistics
- Performance tracking

### UserToken (New)
- OTP management
- Token validation
- Expiration handling

### UserSession (New)
- Session tracking
- Device management
- Activity monitoring

## Benefits

1. **No Duplicate Properties**: Each property has a single source of truth
2. **Better Organization**: Related properties grouped in value objects
3. **Proper Separation of Concerns**: Each entity has a clear responsibility
4. **Improved Security**: Centralized token management with proper validation
5. **Better Tracking**: Comprehensive session and activity tracking
6. **Maintainability**: Cleaner code structure and relationships
7. **Scalability**: Proper entity relationships for future growth

## Migration Considerations

1. **Database Migration**: New tables for UserToken and UserSession
2. **Service Updates**: Services updated to use value objects
3. **Repository Updates**: May need updates for new navigation properties
4. **Configuration Updates**: Entity Framework configurations for new entities

## Next Steps

1. Create Entity Framework configurations for new entities
2. Generate and run database migrations
3. Update repository interfaces and implementations
4. Update DTOs and ViewModels to use new structure
5. Update unit tests to reflect new organization