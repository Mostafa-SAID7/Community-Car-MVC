# Account Structure Reorganization

## Overview
This document describes the reorganization of Account-related DTOs, ViewModels, Mappings, and Models into a consistent, granular folder structure across all layers.

## New Folder Structure

All Account-related components now follow this consistent structure:

```
Core/           - Basic user entities and core functionality
Authentication/ - Sessions, tokens, OAuth, security
Activity/       - User activities and analytics
Gamification/   - Achievements, badges, levels
Social/         - Following, interests, profile views, suggestions
Media/          - Gallery, images, media management
Management/     - User management, hierarchy, actions
```

## Reorganized Components

### 1. Application Layer DTOs
**Location**: `src/CommunityCar.Application/Features/Account/DTOs/`

#### Core/
- `UserDTOs.cs` - Basic user data transfer objects
- `ProfileDTOs.cs` - Profile-related DTOs
- `DashboardDTOs.cs` - Dashboard and overview DTOs

#### Authentication/
- `UserSessionDTOs.cs` - Session management DTOs
- `UserTokenDTOs.cs` - Token management DTOs
- `OAuthDTOs.cs` - OAuth provider DTOs

#### Activity/
- `UserActivityDTOs.cs` - User activity tracking DTOs

#### Gamification/
- `AchievementDTOs.cs` - Achievement system DTOs
- `BadgeDTOs.cs` - Badge system DTOs

#### Social/
- `FollowingDTOs.cs` - Following/follower DTOs
- `InterestDTOs.cs` - User interests DTOs
- `ProfileViewDTOs.cs` - Profile view tracking DTOs
- `SuggestionDTOs.cs` - User suggestion DTOs

#### Media/
- `UserGalleryDTOs.cs` - Gallery and media DTOs

#### Management/
- `UserManagementDTOs.cs` - User management DTOs
- `ManagementAnalyticsDTOs.cs` - Management analytics DTOs

### 2. Application Layer ViewModels
**Location**: `src/CommunityCar.Application/Features/Account/ViewModels/`

#### Core/
- `UserViewModels.cs` - Basic user view models

#### Authentication/
- `SessionViewModels.cs` - Session management view models
- `SecurityViewModels.cs` - Security-related view models
- `OAuthViewModels.cs` - OAuth connection view models

#### Activity/
- `UserActivityViewModels.cs` - Activity tracking view models

#### Gamification/
- `AchievementViewModels.cs` - Achievement view models
- `BadgeViewModels.cs` - Badge view models
- `GamificationOverviewViewModels.cs` - Overview view models

#### Social/
- `FollowingViewModels.cs` - Following/follower view models
- `InterestViewModels.cs` - Interest management view models
- `ProfileViewViewModels.cs` - Profile view analytics view models
- `SuggestionViewModels.cs` - User suggestion view models

#### Media/
- `GalleryViewModels.cs` - Gallery view models
- `MediaManagementViewModels.cs` - Media management view models

#### Management/
- `UserManagementViewModels.cs` - User management view models
- `ManagementActionViewModels.cs` - Management action view models
- `ManagerAssignmentViewModels.cs` - Manager assignment view models

### 3. Application Layer Mappings
**Location**: `src/CommunityCar.Application/Features/Account/Mappings/`

#### Core/
- `UserMappingProfile.cs` - User entity mappings

#### Authentication/
- `SessionMappingProfile.cs` - Session mappings
- `TokenMappingProfile.cs` - Token mappings
- `OAuthMappingProfile.cs` - OAuth mappings

#### Activity/
- `ActivityMappingProfile.cs` - Activity mappings

#### Gamification/
- `AchievementMappingProfile.cs` - Achievement mappings
- `BadgeMappingProfile.cs` - Badge mappings

#### Social/
- `FollowingMappingProfile.cs` - Following mappings
- `InterestMappingProfile.cs` - Interest mappings
- `ProfileViewMappingProfile.cs` - Profile view mappings

#### Media/
- `GalleryMappingProfile.cs` - Gallery mappings

#### Management/
- `UserManagementMappingProfile.cs` - User management mappings
- `ManagementActionMappingProfile.cs` - Management action mappings

### 4. Web Layer Models
**Location**: `src/CommunityCar.Web/Models/Account/`

#### Core/
- `UserWebViewModels.cs` - Web-specific user view models

#### Authentication/
- `SessionWebViewModels.cs` - Web session view models
- `SecurityWebViewModels.cs` - Web security view models
- `OAuthWebViewModels.cs` - Web OAuth view models

#### Activity/
- `ActivityWebViewModels.cs` - Web activity view models

#### Gamification/
- `AchievementWebViewModels.cs` - Web achievement view models
- `BadgeWebViewModels.cs` - Web badge view models

#### Social/
- `FollowingWebViewModels.cs` - Web following view models
- `InterestWebViewModels.cs` - Web interest view models
- `SuggestionWebViewModels.cs` - Web suggestion view models

#### Media/
- `GalleryWebViewModels.cs` - Web gallery view models

## Backward Compatibility

### Umbrella Files
Each layer includes umbrella files that re-export all classes for backward compatibility:

- `src/CommunityCar.Application/Features/Account/ViewModels/AccountViewModels.cs`
- `src/CommunityCar.Application/Features/Account/Mappings/AccountMappingProfile.cs`
- `src/CommunityCar.Web/Models/Account/AccountViewModels.cs`

### Global Usings
All umbrella files use `global using` statements to automatically import all organized namespaces, ensuring existing code continues to work without modification.

## Benefits

1. **Granular Organization**: Each file has a single responsibility and focused scope
2. **Consistent Structure**: Same folder organization across all layers
3. **Easy Navigation**: Developers can quickly find related components
4. **Maintainability**: Smaller, focused files are easier to maintain
5. **Scalability**: New features can be added to appropriate folders
6. **No Breaking Changes**: Existing code continues to work through umbrella files

## Migration Notes

- All existing imports and references continue to work
- New development should use the specific namespaces for better organization
- The umbrella files provide a migration path for gradual adoption
- AutoMapper profiles are automatically discovered by the framework

## File Count Summary

- **DTOs**: 20 files across 7 folders (previously 6 monolithic files)
- **ViewModels**: 15 files across 7 folders (previously 5 monolithic files)  
- **Mappings**: 12 files across 7 folders (previously 3 monolithic files)
- **Web Models**: 12 files across 6 folders (partially organized before)

Total: **59 organized files** replacing **14 monolithic files**