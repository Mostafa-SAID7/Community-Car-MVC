using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.AI;

namespace CommunityCar.Application.Common.Interfaces.Repositories.AI;

public interface ITrainingHistoryRepository : IBaseRepository<TrainingHistory>
{
    Task<IEnumerable<TrainingHistory>> GetHistoryByModelIdAsync(Guid modelId);
    Task<IEnumerable<TrainingHistory>> GetHistoryByJobIdAsync(Guid jobId);
    Task<IEnumerable<TrainingHistory>> GetRecentHistoryAsync(int count = 10);
    Task<IEnumerable<TrainingHistory>> GetSuccessfulTrainingsAsync(Guid modelId);
    Task<IEnumerable<TrainingHistory>> GetFailedTrainingsAsync(Guid modelId);
    Task<TrainingHistory?> GetLatestTrainingAsync(Guid modelId);
    Task<bool> DeleteOldHistoryAsync(DateTime cutoffDate);
}