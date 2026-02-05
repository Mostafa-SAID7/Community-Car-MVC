using CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories.Analytics.Content;
using CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories.Analytics.Users.Behavior;
using CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories.Analytics.Users.Segments;
using CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories.Analytics.Users.Preferences;
using CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories.Management;
using CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories.Management.Users.Core;
using CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories.Management.Users.Actions;
using CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories.Overview.Users.Statistics;
using CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories.Overview.Users.Activity;
using CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories.Overview.Users.Security;
using CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories.Reports.Users.General;
using CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories.Reports.Users.Security;
using CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories.Reports.Users.Audit;
using CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories.Reports;
using CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories.Management.Authorization;

namespace CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories;

public interface IDashboardUnitOfWork : IDisposable
{
    // Authorization
    IRoleRepository Roles { get; }
    IPermissionRepository Permissions { get; }
    
    // Analytics
    IContentAnalyticsRepository ContentAnalytics { get; }
    IUserBehaviorAnalyticsRepository UserBehaviorAnalytics { get; }
    IUserSegmentRepository UserSegments { get; }
    IUserPreferencesRepository UserPreferences { get; }
    
    // Management
    IUserManagementRepository UserManagement { get; }
    IUserManagementActionsRepository UserManagementActions { get; }
    
    // Overview
    IUserOverviewStatisticsRepository UserOverviewStatistics { get; }
    IUserOverviewActivityRepository UserOverviewActivity { get; }
    IUserOverviewSecurityRepository UserOverviewSecurity { get; }
    
    // Reports
    IUserReportsRepository UserReports { get; }
    IUserSecurityReportsRepository UserSecurityReports { get; }
    IUserAuditReportsRepository UserAuditReports { get; }
    IReportRepository Reports { get; }
    IReportScheduleRepository ReportSchedules { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}



