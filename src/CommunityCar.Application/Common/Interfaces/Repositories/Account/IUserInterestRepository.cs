using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Account.Profile;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Account;

/// <summary>
/// Repository interface for UserInterest entity operations
/// </summary>
public interface IUserInterestRepository : IBaseRepository<UserInterest>
{
    #region Interest Management
    Task<IEnumerable<UserInterest>> GetUserInterestsAsync(Guid userId);
    Task<UserInterest?> GetUserInterestAsync(Guid userId, Guid interestId);
    Task<bool> HasInterestAsync(Guid userId, Guid interestId);
    Task<bool> AddInterestAsync(Guid userId, Guid interestId, int priority = 0);
    Task<bool> RemoveInterestAsync(Guid userId, Guid interestId);
    Task<bool> UpdateInterestPriorityAsync(Guid userId, Guid interestId, int priority);
    #endregion

    #region Interest Analytics
    Task<int> GetInterestCountAsync(Guid userId);
    Task<IEnumerable<UserInterest>> GetTopInterestsAsync(Guid userId, int count = 10);
    Task<Dictionary<Guid, int>> GetInterestStatisticsAsync();
    Task<IEnumerable<Guid>> GetUsersWithInterestAsync(Guid interestId, int page = 1, int pageSize = 20);
    Task<IEnumerable<Guid>> GetUsersWithSimilarInterestsAsync(Guid userId, int count = 10);
    #endregion

    #region Interest Categories
    Task<IEnumerable<UserInterest>> GetInterestsByCategoryAsync(Guid userId, string category);
    Task<bool> UpdateInterestCategoryAsync(Guid userId, Guid interestId, string category);
    Task<IEnumerable<string>> GetUserInterestCategoriesAsync(Guid userId);
    #endregion

    #region Interest Recommendations
    Task<IEnumerable<Guid>> GetRecommendedInterestsAsync(Guid userId, int count = 10);
    Task<IEnumerable<Guid>> GetTrendingInterestsAsync(int count = 10);
    Task<double> GetInterestSimilarityAsync(Guid userId1, Guid userId2);
    #endregion
}