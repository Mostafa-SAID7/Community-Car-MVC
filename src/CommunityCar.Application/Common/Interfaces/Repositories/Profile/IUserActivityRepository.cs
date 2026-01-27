using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Profile;
using CommunityCar.Domain.Enums.Users;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Profile;

public interface IUserActivityRepository : IBaseRepository<UserActivity>
{
    Task<IEnumerable<UserActivity>> GetUserActivitiesAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<IEnumerable<UserActivity>> GetUserActivitiesByTypeAsync(Guid userId, ActivityType activityType, int page = 1, int pageSize = 20);
    Task<int> GetUserActivityCountAsync(Guid userId);
    Task<int> GetUserActivityCountByTypeAsync(Guid userId, ActivityType activityType);
    Task<IEnumerable<UserActivity>> GetRecentActivitiesAsync(Guid userId, int count = 10);
    Task<UserActivity?> GetLastActivityAsync(Guid userId);
}


