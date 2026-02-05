using CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories;
using CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories.Analytics.Content;
using CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories.Analytics.Users.Behavior;
using CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories.Analytics.Users.Segments;
using CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories.Analytics.Users.Preferences;
using CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories.Management;
using CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories.Management.Users.Core;
using CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories.Management.Users.Actions;
using CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories.Management.Authorization;
using CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories.Overview.Users.Statistics;
using CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories.Overview.Users.Activity;
using CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories.Overview.Users.Security;
using CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories.Reports.Users.General;
using CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories.Reports.Users.Security;
using CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories.Reports.Users.Audit;
using CommunityCar.Web.Areas.Dashboard.Interfaces.Repositories.Reports;
using CommunityCar.Infrastructure.Persistence.Data;

namespace CommunityCar.Web.Areas.Dashboard.Repositories;

public class DashboardUnitOfWork : IDashboardUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public DashboardUnitOfWork(
        ApplicationDbContext context,
        IContentAnalyticsRepository contentAnalytics,
        IUserBehaviorAnalyticsRepository userBehaviorAnalytics,
        IUserSegmentRepository userSegments,
        IUserPreferencesRepository userPreferences,
        IUserManagementRepository userManagement,
        IUserManagementActionsRepository userManagementActions,
        IUserOverviewStatisticsRepository userOverviewStatistics,
        IUserOverviewActivityRepository userOverviewActivity,
        IUserOverviewSecurityRepository userOverviewSecurity,
        IUserReportsRepository userReports,
        IUserSecurityReportsRepository userSecurityReports,
        IUserAuditReportsRepository userAuditReports,
        IReportRepository reports,
        IReportScheduleRepository reportSchedules,
        IRoleRepository roles,
        IPermissionRepository permissions)
    {
        _context = context;
        ContentAnalytics = contentAnalytics;
        UserBehaviorAnalytics = userBehaviorAnalytics;
        UserSegments = userSegments;
        UserPreferences = userPreferences;
        UserManagement = userManagement;
        UserManagementActions = userManagementActions;
        UserOverviewStatistics = userOverviewStatistics;
        UserOverviewActivity = userOverviewActivity;
        UserOverviewSecurity = userOverviewSecurity;
        UserReports = userReports;
        UserSecurityReports = userSecurityReports;
        UserAuditReports = userAuditReports;
        Reports = reports;
        ReportSchedules = reportSchedules;
        Roles = roles;
        Permissions = permissions;
    }

    public IContentAnalyticsRepository ContentAnalytics { get; }
    public IUserBehaviorAnalyticsRepository UserBehaviorAnalytics { get; }
    public IUserSegmentRepository UserSegments { get; }
    public IUserPreferencesRepository UserPreferences { get; }
    public IUserManagementRepository UserManagement { get; }
    public IUserManagementActionsRepository UserManagementActions { get; }
    public IUserOverviewStatisticsRepository UserOverviewStatistics { get; }
    public IUserOverviewActivityRepository UserOverviewActivity { get; }
    public IUserOverviewSecurityRepository UserOverviewSecurity { get; }
    public IUserReportsRepository UserReports { get; }
    public IUserSecurityReportsRepository UserSecurityReports { get; }
    public IUserAuditReportsRepository UserAuditReports { get; }
    public IReportRepository Reports { get; }
    public IReportScheduleRepository ReportSchedules { get; }
    public IRoleRepository Roles { get; }
    public IPermissionRepository Permissions { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
