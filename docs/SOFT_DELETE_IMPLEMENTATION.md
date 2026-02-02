# Soft Delete Implementation for Community Features

## Overview

This document outlines the comprehensive soft delete implementation for the Community part of the CommunityCar application. The implementation provides a robust system for managing deleted content with the ability to restore or permanently remove items.

## Architecture

### 1. Base Infrastructure

#### ISoftDeletable Interface
- Located in `src/CommunityCar.Domain/Base/IBaseEntity.cs`
- Provides contract for soft delete functionality
- Properties: `IsDeleted`, `DeletedAt`, `DeletedBy`
- Methods: `SoftDelete()`, `Restore()`

#### BaseEntity Implementation
- Located in `src/CommunityCar.Domain/Base/BaseEntity.cs`
- Implements `ISoftDeletable` interface
- All community entities inherit from this base class
- Automatic audit trail for soft delete operations

#### SoftDeleteExtensions
- Located in `src/CommunityCar.Infrastructure/Extensions/SoftDeleteExtensions.cs`
- EF Core query filters for automatic exclusion of deleted items
- Extension methods for including/excluding deleted items in queries
- Bulk operations support

### 2. Repository Layer

#### Enhanced Base Repository
- Updated `IBaseRepository<T>` with soft delete methods:
  - `GetByIdIncludeDeletedAsync()`
  - `GetAllIncludeDeletedAsync()`
  - `GetDeletedOnlyAsync()`
  - `SoftDeleteAsync()` / `RestoreAsync()`
  - `PermanentDeleteAsync()`
  - Bulk operations for all above methods

#### Posts Repository
- Enhanced `IPostsRepository` with community-specific soft delete methods:
  - `GetBySlugIncludeDeletedAsync()`
  - `SearchIncludeDeletedAsync()`
  - `GetByAuthorIncludeDeletedAsync()`
  - `SoftDeletePostsByAuthorAsync()`
  - `GetDeletedPostsForModerationAsync()`

### 3. Service Layer

#### ISoftDeleteService
- Located in `src/CommunityCar.Application/Common/Interfaces/Services/Community/ISoftDeleteService.cs`
- Centralized service for all soft delete operations
- Supports Posts, Stories, Groups, and Comments
- User-specific and admin operations
- Bulk operations and cleanup functionality

#### SoftDeleteService Implementation
- Located in `src/CommunityCar.Application/Services/Community/SoftDeleteService.cs`
- Implements authorization checks
- Comprehensive logging
- Error handling and validation
- Extensible for future content types

#### Enhanced PostsService
- Updated with soft delete methods:
  - `SoftDeletePostAsync()`
  - `RestorePostAsync()`
  - `PermanentDeletePostAsync()`
  - `BulkSoftDeletePostsByAuthorAsync()`
  - `SearchDeletedPostsAsync()`

### 4. View Models

#### Soft Delete View Models
- Located in `src/CommunityCar.Application/Features/Community/SoftDelete/ViewModels/`
- Comprehensive set of request/response models:
  - `SoftDeleteRequestVM` / `SoftDeleteResponseVM`
  - `BulkSoftDeleteRequestVM` / `BulkSoftDeleteResponseVM`
  - `RestoreRequestVM` / `RestoreResponseVM`
  - `DeletedContentSearchVM`
  - `CleanupConfigurationVM` / `CleanupReportVM`

### 5. Controller Layer

#### SoftDeleteController
- Located in `src/CommunityCar.Web/Controllers/Community/SoftDeleteController.cs`
- RESTful API endpoints for all soft delete operations
- Authorization attributes for admin-only operations
- Comprehensive error handling
- Both MVC views and API endpoints

### 6. User Interface

#### Admin Management Interface
- Located in `src/CommunityCar.Web/Views/Community/SoftDelete/Index.cshtml`
- Comprehensive deleted content management
- Filtering and search capabilities
- Bulk operations (restore/permanent delete)
- Cleanup functionality
- Real-time updates with JavaScript

## Features

### 1. Content Management
- **Soft Delete**: Mark content as deleted without removing from database
- **Restore**: Restore previously deleted content
- **Permanent Delete**: Completely remove content from database (admin only)
- **Bulk Operations**: Process multiple items simultaneously

### 2. User Permissions
- **Users**: Can soft delete and restore their own content
- **Moderators**: Can manage any content
- **Admins**: Full access including permanent deletion and cleanup

### 3. Content Types Supported
- **Posts**: Full implementation with enhanced repository
- **Stories**: Interface ready (implementation pending)
- **Groups**: Interface ready (implementation pending)
- **Comments**: Interface ready (implementation pending)

### 4. Admin Features
- **Statistics Dashboard**: View deleted content statistics
- **Cleanup Tools**: Automatically remove old deleted content
- **User Management**: Bulk operations on user content
- **Audit Trail**: Complete history of delete/restore operations

### 5. Search and Filtering
- **Content Search**: Search within deleted content
- **Type Filtering**: Filter by content type (Post, Story, Group, etc.)
- **Date Filtering**: Filter by deletion date range
- **Author Filtering**: Filter by content author
- **Advanced Sorting**: Multiple sort options

## Usage Examples

### 1. Soft Delete a Post
```csharp
// In a controller or service
var result = await _softDeleteService.SoftDeletePostAsync(postId);
if (result)
{
    // Post successfully soft deleted
    // User can still restore it
}
```

### 2. Restore Deleted Content
```csharp
// Restore a previously deleted post
var result = await _softDeleteService.RestorePostAsync(postId);
if (result)
{
    // Post is now visible again
}
```

### 3. Bulk Operations
```csharp
// Soft delete multiple posts
var postIds = new[] { id1, id2, id3 };
var deletedCount = await _softDeleteService.BulkSoftDeletePostsAsync(postIds);
```

### 4. Admin Cleanup
```csharp
// Remove content deleted more than 30 days ago
var cleanedCount = await _softDeleteService.CleanupOldDeletedContentAsync(30);
```

### 5. Search Deleted Content
```csharp
// Get deleted posts for current user
var deletedPosts = await _softDeleteService.GetDeletedPostsAsync(page: 1, pageSize: 20);
```

## Security Considerations

### 1. Authorization
- Users can only manage their own content
- Moderators can manage any content
- Admins have full access including permanent deletion
- Permission-based access control using `[RequirePermission]` attributes

### 2. Audit Trail
- All operations are logged with user information
- Deletion reason can be recorded
- Complete history of who deleted/restored what and when

### 3. Data Protection
- Soft deleted content is hidden from normal queries
- Only authorized users can access deleted content
- Permanent deletion requires explicit confirmation

## Configuration

### 1. Dependency Injection
The soft delete service is registered in `DependencyInjection.cs`:
```csharp
services.AddScoped<ISoftDeleteService, SoftDeleteService>();
```

### 2. Entity Framework Configuration
Soft delete query filters are automatically applied in `DbContext`:
```csharp
modelBuilder.ConfigureSoftDeleteFilter();
```

### 3. Permissions
Required permissions for different operations:
- `Admin.ManageDeletedContent`: View deleted content management
- `Admin.PermanentDeleteContent`: Permanently delete content
- `Admin.BulkDeleteContent`: Bulk soft delete operations
- `Admin.CleanupDeletedContent`: Cleanup old deleted content

## Future Enhancements

### 1. Scheduled Cleanup
- Implement background job for automatic cleanup
- Configurable retention periods
- Email notifications before cleanup

### 2. Content Recovery
- Enhanced recovery options
- Batch recovery tools
- Recovery approval workflow

### 3. Analytics
- Deletion pattern analysis
- Content lifecycle metrics
- User behavior insights

### 4. Integration
- Extend to other content types (Comments, Reviews, etc.)
- Integration with content moderation system
- API versioning for external integrations

## Testing

### 1. Unit Tests
- Service layer tests for all operations
- Repository tests for data access
- Authorization tests for security

### 2. Integration Tests
- End-to-end workflow tests
- Database integration tests
- API endpoint tests

### 3. Performance Tests
- Bulk operation performance
- Query filter performance
- Cleanup operation efficiency

## Monitoring and Logging

### 1. Application Logging
- All operations are logged with appropriate levels
- Error logging with stack traces
- Performance metrics logging

### 2. Audit Logging
- User action audit trail
- Administrative action logging
- Security event logging

### 3. Metrics
- Deletion/restoration rates
- Content lifecycle metrics
- System performance metrics

## Conclusion

This soft delete implementation provides a comprehensive, secure, and user-friendly system for managing deleted content in the Community features. It balances user needs for content recovery with administrative requirements for content management and system maintenance.

The modular design allows for easy extension to other content types and integration with existing systems while maintaining high performance and security standards.