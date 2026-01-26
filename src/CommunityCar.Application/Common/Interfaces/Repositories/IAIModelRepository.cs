using CommunityCar.Domain.Entities.AI;

namespace CommunityCar.Application.Common.Interfaces.Repositories;

public interface IAIModelRepository
{
    Task<IEnumerable<AIModel>> GetAllModelsAsync();
    Task<AIModel?> GetModelByIdAsync(Guid id);
    Task<AIModel?> GetModelByNameAsync(string name);
    Task<IEnumerable<AIModel>> GetActiveModelsAsync();
    Task<IEnumerable<AIModel>> GetModelsByTypeAsync(AIModelType type);
    Task<AIModel> CreateModelAsync(AIModel model);
    Task<AIModel> UpdateModelAsync(AIModel model);
    Task<bool> DeleteModelAsync(Guid id);
    Task<bool> ModelExistsAsync(string name);
}

public interface ITrainingJobRepository
{
    Task<IEnumerable<TrainingJob>> GetAllJobsAsync();
    Task<IEnumerable<TrainingJob>> GetQueuedJobsAsync();
    Task<IEnumerable<TrainingJob>> GetJobsByModelIdAsync(Guid modelId);
    Task<TrainingJob?> GetJobByIdAsync(Guid id);
    Task<TrainingJob> CreateJobAsync(TrainingJob job);
    Task<TrainingJob> UpdateJobAsync(TrainingJob job);
    Task<bool> DeleteJobAsync(Guid id);
}

public interface ITrainingHistoryRepository
{
    Task<IEnumerable<TrainingHistory>> GetHistoryByModelIdAsync(Guid modelId);
    Task<IEnumerable<TrainingHistory>> GetRecentHistoryAsync(int count = 10);
    Task<TrainingHistory?> GetHistoryByIdAsync(Guid id);
    Task<TrainingHistory> CreateHistoryAsync(TrainingHistory history);
    Task<TrainingHistory> UpdateHistoryAsync(TrainingHistory history);
    Task<bool> DeleteHistoryAsync(Guid id);
}