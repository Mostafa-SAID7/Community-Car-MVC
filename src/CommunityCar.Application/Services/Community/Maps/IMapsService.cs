using CommunityCar.Application.Features.Community.Maps.ViewModels;

namespace CommunityCar.Application.Services.Community.Maps;

public interface IMapsService
{
    Task<List<PointOfInterestVM>> GetPointsOfInterestAsync(double latitude, double longitude, double radius);
    Task<PointOfInterestVM> CreatePointOfInterestAsync(CreatePointOfInterestVM model);
    Task<PointOfInterestVM?> UpdatePointOfInterestAsync(Guid id, CreatePointOfInterestVM model);
    Task<bool> DeletePointOfInterestAsync(Guid id);
    Task<List<CommunityReportDetailsVM>> GetCommunityReportsAsync(double latitude, double longitude, double radius);
    Task<CommunityReportDetailsVM> CreateCommunityReportAsync(CommunityReportDetailsVM model);
    Task<bool> ResolveCommunityReportAsync(Guid id, string resolution);
}