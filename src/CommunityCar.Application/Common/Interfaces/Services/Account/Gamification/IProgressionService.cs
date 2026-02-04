using CommunityCar.Application.Features.Account.ViewModels.Gamification;

namespace CommunityCar.Application.Common.Interfaces.Services.Account.Gamification;

public interface IProgressionService
{
    Task<UserProgressionVM> GetUserProgressionAsync(Guid userId);
    Task<bool> UpdateUserProgressionAsync(Guid userId, string activityType, int points = 1);
    Task<List<LevelVM>> GetAvailableLevelsAsync();
    Task<LevelVM> GetUserCurrentLevelAsync(Guid userId);
    Task<LevelVM?> GetNextLevelAsync(Guid userId);
    Task<bool> CheckLevelUpAsync(Guid userId);
    Task<List<ProgressionHistoryVM>> GetProgressionHistoryAsync(Guid userId, int page = 1, int pageSize = 20);
}