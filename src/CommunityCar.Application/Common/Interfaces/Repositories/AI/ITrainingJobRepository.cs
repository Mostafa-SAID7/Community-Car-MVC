using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.AI;

namespace CommunityCar.Application.Common.Interfaces.Repositories.AI;

public interface ITrainingJobRepository : IBaseRepository<TrainingJob>
{
    Task<IEnumerable<TrainingJob>> GetQueuedJobsAsync();
    Task<IEnumerable<TrainingJob>> GetRunningJobsAsync();
    Task<IEnumerable<TrainingJob>> GetCompletedJobsAsync();
    Task<IEnumerable<TrainingJob>> GetJobsByModelIdAsync(Guid modelId);
    Task<IEnumerable<TrainingJob>> GetJobsByUserAsync(Guid userId);
    Task<bool> StartJobAsync(Guid jobId);
    Task<bool> CompleteJobAsync(Guid jobId, bool success, string? errorMessage = null);
    Task<bool> CancelJobAsync(Guid jobId);
    Task<int> GetQueuePositionAsync(Guid jobId);
}