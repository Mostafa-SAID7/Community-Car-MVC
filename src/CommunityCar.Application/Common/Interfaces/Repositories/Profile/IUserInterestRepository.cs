using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Account.Profile;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Profile;

public interface IUserInterestRepository : IBaseRepository<UserInterest>
{
    Task<IEnumerable<UserInterest>> GetUserInterestsAsync(Guid userId, bool activeOnly = true);
    Task<UserInterest?> GetUserInterestAsync(Guid userId, string category, string subCategory);
    Task<IEnumerable<UserInterest>> GetInterestsByCategoryAsync(Guid userId, string category);
    Task<IEnumerable<UserInterest>> GetTopInterestsAsync(Guid userId, int count = 10);
    Task<Dictionary<string, float>> GetInterestScoresAsync(Guid userId);
    Task<IEnumerable<UserInterest>> GetSimilarInterestsAsync(Guid userId1, Guid userId2);
    Task<IEnumerable<UserInterest>> GetStaleInterestsAsync(Guid userId, DateTime cutoffDate);
}


