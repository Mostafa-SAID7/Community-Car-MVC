using CommunityCar.Application.Features.Dashboard.Analytics.Users.Segments;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Analytics.Users.Segments;

public interface IUserSegmentRepository
{
    Task<List<UserSegmentVM>> GetUserSegmentsAsync();
    Task<UserSegmentVM?> GetUserSegmentByIdAsync(int segmentId);
    Task<UserSegmentAnalyticsVM> GetSegmentAnalyticsAsync(int segmentId);
    Task<List<UserSegmentTrendVM>> GetSegmentTrendsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<List<string>> GetUsersInSegmentAsync(int segmentId);
    Task<int> CreateUserSegmentAsync(UserSegmentVM segment);
    Task<bool> UpdateUserSegmentAsync(UserSegmentVM segment);
    Task<bool> DeleteUserSegmentAsync(int segmentId);
}