namespace CommunityCar.Application.Common.Interfaces.Services.Account;

public interface IProgressionService
{
    Task<bool> CanCreateGuideAsync(Guid userId);
    Task<bool> CanCreateReviewAsync(Guid userId);
    Task<bool> CanCreateArticleAsync(Guid userId);
    
    Task<int> GetRemainingGuidesTodayAsync(Guid userId);
    Task<int> GetRemainingReviewsTodayAsync(Guid userId);
    Task<int> GetRemainingArticlesTodayAsync(Guid userId);

    Task<bool> RequestAdminStatusAsync(Guid userId);
}
