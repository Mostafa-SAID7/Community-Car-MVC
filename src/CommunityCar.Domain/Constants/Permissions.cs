namespace CommunityCar.Domain.Constants;

/// <summary>
/// System permissions organized by category
/// </summary>
public static class Permissions
{
    // User Management Permissions
    public static class Users
    {
        public const string View = "users.view";
        public const string Create = "users.create";
        public const string Edit = "users.edit";
        public const string Delete = "users.delete";
        public const string ViewProfile = "users.view_profile";
        public const string EditProfile = "users.edit_profile";
        public const string ViewSessions = "users.view_sessions";
        public const string ManageSessions = "users.manage_sessions";
        public const string ViewActivities = "users.view_activities";
        public const string Impersonate = "users.impersonate";
        public const string Export = "users.export";
    }

    // Role Management Permissions
    public static class Roles
    {
        public const string View = "roles.view";
        public const string Create = "roles.create";
        public const string Edit = "roles.edit";
        public const string Delete = "roles.delete";
        public const string Assign = "roles.assign";
        public const string Unassign = "roles.unassign";
        public const string ViewPermissions = "roles.view_permissions";
        public const string ManagePermissions = "roles.manage_permissions";
    }

    // Permission Management Permissions
    public static class PermissionManagement
    {
        public const string View = "permissions.view";
        public const string Create = "permissions.create";
        public const string Edit = "permissions.edit";
        public const string Delete = "permissions.delete";
        public const string Grant = "permissions.grant";
        public const string Revoke = "permissions.revoke";
        public const string ViewAudit = "permissions.view_audit";
    }

    // Content Management Permissions
    public static class Content
    {
        public const string View = "content.view";
        public const string Create = "content.create";
        public const string Edit = "content.edit";
        public const string Delete = "content.delete";
        public const string Publish = "content.publish";
        public const string Unpublish = "content.unpublish";
        public const string Moderate = "content.moderate";
        public const string Feature = "content.feature";
        public const string Verify = "content.verify";
        public const string ViewDrafts = "content.view_drafts";
        public const string EditOthers = "content.edit_others";
        public const string DeleteOthers = "content.delete_others";
    }

    // Community Management Permissions
    public static class Community
    {
        public const string ViewGroups = "community.view_groups";
        public const string CreateGroups = "community.create_groups";
        public const string ManageGroups = "community.manage_groups";
        public const string DeleteGroups = "community.delete_groups";
        public const string ViewEvents = "community.view_events";
        public const string CreateEvents = "community.create_events";
        public const string ManageEvents = "community.manage_events";
        public const string DeleteEvents = "community.delete_events";
        public const string ModerateComments = "community.moderate_comments";
        public const string BanUsers = "community.ban_users";
        public const string ViewReports = "community.view_reports";
        public const string HandleReports = "community.handle_reports";
    }

    // System Administration Permissions
    public static class System
    {
        public const string ViewLogs = "system.view_logs";
        public const string ViewMetrics = "system.view_metrics";
        public const string ViewDashboard = "system.view_dashboard";
        public const string ManageSettings = "system.manage_settings";
        public const string ManageCache = "system.manage_cache";
        public const string ManageJobs = "system.manage_jobs";
        public const string DatabaseAccess = "system.database_access";
        public const string SystemConfiguration = "system.configuration";
        public const string BackupRestore = "system.backup_restore";
        public const string MaintenanceMode = "system.maintenance_mode";
    }

    // Security Permissions
    public static class Security
    {
        public const string ViewSecurityLogs = "security.view_logs";
        public const string ManageTwoFactor = "security.manage_2fa";
        public const string ViewSessions = "security.view_sessions";
        public const string ManageSessions = "security.manage_sessions";
        public const string UnlockAccounts = "security.unlock_accounts";
        public const string ResetPasswords = "security.reset_passwords";
        public const string ViewAuditTrail = "security.view_audit";
        public const string ManageSecuritySettings = "security.manage_settings";
    }

    // Analytics Permissions
    public static class Analytics
    {
        public const string ViewBasic = "analytics.view_basic";
        public const string ViewAdvanced = "analytics.view_advanced";
        public const string ViewUserAnalytics = "analytics.view_users";
        public const string ViewContentAnalytics = "analytics.view_content";
        public const string ViewSystemAnalytics = "analytics.view_system";
        public const string ExportReports = "analytics.export_reports";
        public const string CreateReports = "analytics.create_reports";
    }

    // AI Management Permissions
    public static class AI
    {
        public const string ViewModels = "ai.view_models";
        public const string ManageModels = "ai.manage_models";
        public const string TrainModels = "ai.train_models";
        public const string ViewTraining = "ai.view_training";
        public const string ManageTraining = "ai.manage_training";
        public const string ViewPredictions = "ai.view_predictions";
        public const string ConfigureAI = "ai.configure";
    }

    // Media Management Permissions
    public static class Media
    {
        public const string View = "media.view";
        public const string Upload = "media.upload";
        public const string Edit = "media.edit";
        public const string Delete = "media.delete";
        public const string ViewOthers = "media.view_others";
        public const string EditOthers = "media.edit_others";
        public const string DeleteOthers = "media.delete_others";
        public const string ManageStorage = "media.manage_storage";
    }

    // API Access Permissions
    public static class API
    {
        public const string Read = "api.read";
        public const string Write = "api.write";
        public const string Admin = "api.admin";
        public const string ViewKeys = "api.view_keys";
        public const string ManageKeys = "api.manage_keys";
        public const string ViewUsage = "api.view_usage";
    }

    /// <summary>
    /// Get all permissions organized by category
    /// </summary>
    public static Dictionary<string, List<string>> GetAllPermissions()
    {
        return new Dictionary<string, List<string>>
        {
            ["Users"] = new List<string>
            {
                Users.View, Users.Create, Users.Edit, Users.Delete,
                Users.ViewProfile, Users.EditProfile, Users.ViewSessions,
                Users.ManageSessions, Users.ViewActivities, Users.Impersonate, Users.Export
            },
            ["Roles"] = new List<string>
            {
                Roles.View, Roles.Create, Roles.Edit, Roles.Delete,
                Roles.Assign, Roles.Unassign, Roles.ViewPermissions, Roles.ManagePermissions
            },
            ["Permissions"] = new List<string>
            {
                PermissionManagement.View, PermissionManagement.Create, PermissionManagement.Edit,
                PermissionManagement.Delete, PermissionManagement.Grant, PermissionManagement.Revoke,
                PermissionManagement.ViewAudit
            },
            ["Content"] = new List<string>
            {
                Content.View, Content.Create, Content.Edit, Content.Delete,
                Content.Publish, Content.Unpublish, Content.Moderate, Content.Feature,
                Content.Verify, Content.ViewDrafts, Content.EditOthers, Content.DeleteOthers
            },
            ["Community"] = new List<string>
            {
                Community.ViewGroups, Community.CreateGroups, Community.ManageGroups, Community.DeleteGroups,
                Community.ViewEvents, Community.CreateEvents, Community.ManageEvents, Community.DeleteEvents,
                Community.ModerateComments, Community.BanUsers, Community.ViewReports, Community.HandleReports
            },
            ["System"] = new List<string>
            {
                System.ViewLogs, System.ViewMetrics, System.ViewDashboard, System.ManageSettings,
                System.ManageCache, System.ManageJobs, System.DatabaseAccess, System.SystemConfiguration,
                System.BackupRestore, System.MaintenanceMode
            },
            ["Security"] = new List<string>
            {
                Security.ViewSecurityLogs, Security.ManageTwoFactor, Security.ViewSessions,
                Security.ManageSessions, Security.UnlockAccounts, Security.ResetPasswords,
                Security.ViewAuditTrail, Security.ManageSecuritySettings
            },
            ["Analytics"] = new List<string>
            {
                Analytics.ViewBasic, Analytics.ViewAdvanced, Analytics.ViewUserAnalytics,
                Analytics.ViewContentAnalytics, Analytics.ViewSystemAnalytics, Analytics.ExportReports,
                Analytics.CreateReports
            },
            ["AI"] = new List<string>
            {
                AI.ViewModels, AI.ManageModels, AI.TrainModels, AI.ViewTraining,
                AI.ManageTraining, AI.ViewPredictions, AI.ConfigureAI
            },
            ["Media"] = new List<string>
            {
                Media.View, Media.Upload, Media.Edit, Media.Delete,
                Media.ViewOthers, Media.EditOthers, Media.DeleteOthers, Media.ManageStorage
            },
            ["API"] = new List<string>
            {
                API.Read, API.Write, API.Admin, API.ViewKeys,
                API.ManageKeys, API.ViewUsage
            }
        };
    }

    /// <summary>
    /// Get all permissions as a flat list
    /// </summary>
    public static List<string> GetAllPermissionsList()
    {
        return GetAllPermissions().SelectMany(kvp => kvp.Value).ToList();
    }
}