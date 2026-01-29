# Permissions System Documentation

## Overview

The CommunityCar application now includes a comprehensive Role-Based Access Control (RBAC) system with granular permissions. This system extends the existing ASP.NET Core Identity roles with a flexible permission framework.

## Architecture

### Core Components

1. **Entities**
   - `Permission` - Represents individual permissions
   - `Role` - Extends IdentityRole with additional properties
   - `RolePermission` - Junction table for role-permission relationships
   - `UserPermission` - Direct user permissions (overrides role permissions)

2. **Services**
   - `IPermissionService` - Manages permissions and user/role permission assignments
   - `IRoleService` - Manages roles and role assignments
   - `IPermissionRepository` - Data access for permissions
   - `IRoleRepository` - Data access for roles

3. **Authorization**
   - `RequirePermissionAttribute` - Controller/action authorization
   - `RequireAnyPermissionAttribute` - Requires ANY of specified permissions
   - `RequireAllPermissionsAttribute` - Requires ALL specified permissions
   - `PermissionExtensions` - View helpers for permission checks

## Permission Categories

The system organizes permissions into logical categories:

### Users
- `users.view` - View user information
- `users.create` - Create new users
- `users.edit` - Edit user information
- `users.delete` - Delete users
- `users.view_profile` - View user profiles
- `users.edit_profile` - Edit user profiles
- `users.view_sessions` - View user sessions
- `users.manage_sessions` - Manage user sessions
- `users.view_activities` - View user activities
- `users.impersonate` - Impersonate other users
- `users.export` - Export user data

### Roles
- `roles.view` - View roles
- `roles.create` - Create new roles
- `roles.edit` - Edit roles
- `roles.delete` - Delete roles
- `roles.assign` - Assign roles to users
- `roles.unassign` - Remove roles from users
- `roles.view_permissions` - View role permissions
- `roles.manage_permissions` - Manage role permissions

### Content
- `content.view` - View content
- `content.create` - Create content
- `content.edit` - Edit content
- `content.delete` - Delete content
- `content.publish` - Publish content
- `content.unpublish` - Unpublish content
- `content.moderate` - Moderate content
- `content.feature` - Feature content
- `content.verify` - Verify content
- `content.view_drafts` - View draft content
- `content.edit_others` - Edit others' content
- `content.delete_others` - Delete others' content

### Community
- `community.view_groups` - View community groups
- `community.create_groups` - Create community groups
- `community.manage_groups` - Manage community groups
- `community.delete_groups` - Delete community groups
- `community.view_events` - View events
- `community.create_events` - Create events
- `community.manage_events` - Manage events
- `community.delete_events` - Delete events
- `community.moderate_comments` - Moderate comments
- `community.ban_users` - Ban users from community
- `community.view_reports` - View user reports
- `community.handle_reports` - Handle user reports

### System
- `system.view_logs` - View system logs
- `system.view_metrics` - View system metrics
- `system.view_dashboard` - View admin dashboard
- `system.manage_settings` - Manage system settings
- `system.manage_cache` - Manage system cache
- `system.manage_jobs` - Manage background jobs
- `system.database_access` - Access database directly
- `system.configuration` - System configuration
- `system.backup_restore` - Backup and restore system
- `system.maintenance_mode` - Enable maintenance mode

### Security
- `security.view_logs` - View security logs
- `security.manage_2fa` - Manage two-factor authentication
- `security.view_sessions` - View user sessions
- `security.manage_sessions` - Manage user sessions
- `security.unlock_accounts` - Unlock user accounts
- `security.reset_passwords` - Reset user passwords
- `security.view_audit` - View audit trail
- `security.manage_settings` - Manage security settings

### Analytics
- `analytics.view_basic` - View basic analytics
- `analytics.view_advanced` - View advanced analytics
- `analytics.view_users` - View user analytics
- `analytics.view_content` - View content analytics
- `analytics.view_system` - View system analytics
- `analytics.export_reports` - Export analytics reports
- `analytics.create_reports` - Create custom reports

### AI Management
- `ai.view_models` - View AI models
- `ai.manage_models` - Manage AI models
- `ai.train_models` - Train AI models
- `ai.view_training` - View training progress
- `ai.manage_training` - Manage training jobs
- `ai.view_predictions` - View AI predictions
- `ai.configure` - Configure AI settings

### Media
- `media.view` - View media files
- `media.upload` - Upload media files
- `media.edit` - Edit media files
- `media.delete` - Delete media files
- `media.view_others` - View others' media
- `media.edit_others` - Edit others' media
- `media.delete_others` - Delete others' media
- `media.manage_storage` - Manage media storage

### API Access
- `api.read` - Read API access
- `api.write` - Write API access
- `api.admin` - Admin API access
- `api.view_keys` - View API keys
- `api.manage_keys` - Manage API keys
- `api.view_usage` - View API usage

## System Roles

The system includes predefined roles with appropriate permissions:

### SuperAdmin
- **Priority**: 1000
- **Permissions**: All permissions
- **Description**: Super Administrator with full system access

### Admin
- **Priority**: 900
- **Permissions**: Broad system access excluding super admin functions
- **Description**: Administrator with broad system access

### ContentAdmin
- **Priority**: 800
- **Permissions**: Full content and media management
- **Description**: Content Administrator

### DatabaseAdmin
- **Priority**: 850
- **Permissions**: Database access and system maintenance
- **Description**: Database Administrator

### DesignAdmin
- **Priority**: 700
- **Permissions**: Content editing and media management
- **Description**: Design Administrator

### Master
- **Priority**: 500
- **Permissions**: Advanced content creation and community management
- **Description**: Master User - Highest community level

### Author
- **Priority**: 400
- **Permissions**: Content creation and publishing
- **Description**: Content Author

### Reviewer
- **Priority**: 300
- **Permissions**: Content review and moderation
- **Description**: Content Reviewer

### Expert
- **Priority**: 200
- **Permissions**: Advanced content creation and community participation
- **Description**: Expert User

### User
- **Priority**: 100
- **Permissions**: Basic content viewing and creation
- **Description**: Regular User

## Usage Examples

### Controller Authorization

```csharp
[RequirePermission(Permissions.Users.View)]
public class UsersController : Controller
{
    [RequirePermission(Permissions.Users.Edit)]
    public async Task<IActionResult> Edit(Guid id)
    {
        // Only users with users.edit permission can access
    }

    [RequireAnyPermission(Permissions.Users.View, Permissions.Users.ViewProfile)]
    public async Task<IActionResult> Profile(Guid id)
    {
        // Users with either permission can access
    }
}
```

### View Authorization

```html
@if (await Html.HasPermissionAsync(Permissions.Users.Edit))
{
    <a href="/Users/Edit/@Model.Id" class="btn btn-primary">Edit User</a>
}

@if (await Html.HasAnyPermissionAsync(Permissions.Content.Edit, Permissions.Content.Moderate))
{
    <div class="admin-panel">
        <!-- Admin content -->
    </div>
}
```

### Service Usage

```csharp
// Check permissions
var hasPermission = await _permissionService.HasPermissionAsync(userId, Permissions.Users.Edit);
var hasAnyPermission = await _permissionService.HasAnyPermissionAsync(userId, 
    Permissions.Content.Edit, Permissions.Content.Moderate);

// Grant permissions
await _permissionService.GrantUserPermissionAsync(userId, Permissions.Users.Edit, "Admin", "Manual grant");

// Assign roles
await _roleService.AssignRoleAsync(userId, Roles.Author, "Admin");

// Get user permissions
var userPermissions = await _permissionService.GetUserPermissionsAsync(userId);
```

## Database Schema

### Permissions Table
- Id (Guid, PK)
- Name (string, unique)
- DisplayName (string)
- Description (string, nullable)
- Category (string)
- IsSystemPermission (bool)
- IsActive (bool)
- CreatedAt, UpdatedAt, CreatedBy, UpdatedBy

### Roles Table (extends AspNetRoles)
- Id (Guid, PK)
- Name (string, unique)
- Description (string, nullable)
- Category (string)
- IsSystemRole (bool)
- IsActive (bool)
- Priority (int)
- CreatedAt, UpdatedAt, CreatedBy, UpdatedBy

### RolePermissions Table
- Id (Guid, PK)
- RoleId (Guid, FK)
- PermissionId (Guid, FK)
- IsGranted (bool)
- ExpiresAt (DateTime, nullable)
- GrantedBy (string, nullable)
- Reason (string, nullable)
- CreatedAt, UpdatedAt

### UserPermissions Table
- Id (Guid, PK)
- UserId (Guid, FK)
- PermissionId (Guid, FK)
- IsGranted (bool)
- ExpiresAt (DateTime, nullable)
- GrantedBy (string, nullable)
- Reason (string, nullable)
- IsOverride (bool)
- CreatedAt, UpdatedAt

## Initialization

The system automatically initializes with:

1. **System Permissions**: All predefined permissions are created
2. **System Roles**: All predefined roles with appropriate permissions
3. **Default Assignments**: SuperAdmin role gets all permissions

To initialize the system:

```csharp
// Initialize permissions
await _permissionService.InitializeSystemPermissionsAsync();

// Initialize roles
await _roleService.InitializeSystemRolesAsync();
```

## Best Practices

1. **Use Specific Permissions**: Always use the most specific permission required
2. **Group Related Permissions**: Use categories to organize permissions logically
3. **Document Custom Permissions**: Document any custom permissions you create
4. **Regular Audits**: Regularly audit user permissions and role assignments
5. **Principle of Least Privilege**: Grant only the minimum permissions required
6. **Use Roles for Common Patterns**: Create roles for common permission combinations
7. **Monitor Permission Usage**: Track which permissions are actually used

## Security Considerations

1. **System Permissions**: System permissions cannot be deleted or modified
2. **System Roles**: System roles have limited editing capabilities
3. **Permission Expiration**: Permissions can have expiration dates
4. **Audit Trail**: All permission changes should be logged
5. **Override Permissions**: User permissions can override role permissions
6. **Session Validation**: Permissions are checked on each request
7. **Caching**: Consider caching user permissions for performance

## Migration from Existing System

The new permission system is designed to work alongside the existing role system:

1. **Existing Roles**: All existing roles are preserved and enhanced
2. **Backward Compatibility**: Existing role checks continue to work
3. **Gradual Migration**: You can gradually migrate to permission-based checks
4. **Dual System**: Both role and permission checks can coexist

## API Endpoints

### Permissions Management
- `GET /Dashboard/System/Permissions` - List all permissions
- `GET /Dashboard/System/Permissions/{id}` - Get permission details
- `POST /Dashboard/System/Permissions` - Create permission
- `PUT /Dashboard/System/Permissions/{id}` - Update permission
- `DELETE /Dashboard/System/Permissions/{id}` - Delete permission

### Roles Management
- `GET /Dashboard/System/Roles` - List all roles
- `GET /Dashboard/System/Roles/{id}` - Get role details
- `POST /Dashboard/System/Roles` - Create role
- `PUT /Dashboard/System/Roles/{id}` - Update role
- `DELETE /Dashboard/System/Roles/{id}` - Delete role

### User Management
- `GET /Dashboard/Users/{id}/Permissions` - Get user permissions
- `POST /Dashboard/Users/{id}/Permissions` - Grant user permission
- `DELETE /Dashboard/Users/{id}/Permissions/{permission}` - Revoke user permission
- `GET /Dashboard/Users/{id}/Roles` - Get user roles
- `POST /Dashboard/Users/{id}/Roles` - Assign user role
- `DELETE /Dashboard/Users/{id}/Roles/{role}` - Remove user role

This comprehensive permissions system provides fine-grained access control while maintaining flexibility and ease of use.