using CommunityCar.Domain.Enums.Community;
using CommunityCar.Application.Features.Community.Maps.ViewModels;

namespace CommunityCar.Application.Services.Maps.Community;

public interface ICommunityReportingService
{
    Task<CommunityReportVM> SubmitReportAsync(SubmitCommunityReportVM request);
    Task<bool> ConfirmReportAsync(Guid reportId, Guid userId, bool isConfirmed, string? comment = null);
    Task<List<CommunityReportVM>> GetNearbyReportsAsync(double latitude, double longitude, double radiusKm = 5);
    Task<List<CommunityReportVM>> GetUserReportsAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<bool> ModerateReportAsync(Guid reportId, Guid moderatorId, CommunityReportStatus status, string? reason = null);
    Task<double> GetUserReputationScoreAsync(Guid userId);
    Task UpdateReputationScoresAsync();
}




