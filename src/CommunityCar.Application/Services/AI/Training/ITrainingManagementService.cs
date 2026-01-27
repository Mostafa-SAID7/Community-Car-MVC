using CommunityCar.Domain.Entities.AI;

namespace CommunityCar.Application.Services.AI.Training;

/// <summary>
/// Interface for AI training management operations
/// </summary>
public interface ITrainingManagementService
{
    Task<IEnumerable<TrainingJob>> GetTrainingQueueAsync();
    Task<TrainingJob> StartTrainingAsync(Guid modelId, object parameters);
    Task<TrainingJob> RetrainModelAsync(string modelName, object parameters);
}


