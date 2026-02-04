using CommunityCar.Application.Features.Dashboard.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard;

public interface ISecurityService
{
    Task<SecurityOverviewVM> GetSecurityOverviewAsync();
    Task<List<SecurityThreatVM>> GetSecurityThreatsAsync(int limit = 50);
    Task<List<SecurityEventVM>> GetSecurityEventsAsync(DateTime startDate, DateTime endDate);
    Task<List<FailedLoginAttemptVM>> GetFailedLoginAttemptsAsync(int limit = 100);
    Task<List<SuspiciousActivityVM>> GetSuspiciousActivitiesAsync(int limit = 50);
    Task<SecurityAuditVM> GetSecurityAuditAsync();
    Task<List<VulnerabilityVM>> GetVulnerabilitiesAsync();
    Task<bool> BlockIpAddressAsync(string ipAddress, string reason);
    Task<bool> UnblockIpAddressAsync(string ipAddress);
    Task<List<BlockedIpVM>> GetBlockedIpsAsync();
    Task<bool> EnableTwoFactorForUserAsync(Guid userId);
    Task<bool> DisableTwoFactorForUserAsync(Guid userId);
    Task<SecuritySettingsVM> GetSecuritySettingsAsync();
    Task<bool> UpdateSecuritySettingsAsync(SecuritySettingsVM settings);
    Task<List<ChartDataVM>> GetSecurityMetricsChartAsync(int days);
}