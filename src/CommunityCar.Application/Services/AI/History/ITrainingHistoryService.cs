using CommunityCar.Domain.Entities.AI;

namespace CommunityCar.Application.Services.AI.History;

/// <summary>
/// Interface for training history operations
/// </summary>
public interface ITrainingHistoryService
{
    Task<IEnumerable<TrainingHistory>> GetRecentTrainingHistoryAsync(int count = 10);
    Task<TrainingHistory?> GetTrainingDetailsAsync(string modelName, DateTime date);
    Task<string> GenerateTrainingReportAsync(string modelName, DateTime date);
}


