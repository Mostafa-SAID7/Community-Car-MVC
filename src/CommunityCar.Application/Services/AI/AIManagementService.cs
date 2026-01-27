using CommunityCar.Application.Services.AI.ModelManagement;
using CommunityCar.Application.Services.AI.Training;
using CommunityCar.Application.Services.AI.History;
using CommunityCar.Domain.Entities.AI;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.AI;

public interface IAIManagementService
{
    Task<IEnumerable<AIModel>> GetAllModelsAsync();
    Task<IEnumerable<TrainingJob>> GetTrainingQueueAsync();
    Task<IEnumerable<TrainingHistory>> GetRecentTrainingHistoryAsync(int count = 10);
    Task<AIModel?> GetModelByIdAsync(Guid id);
    Task<TrainingJob> StartTrainingAsync(Guid modelId, object parameters);
    Task<TrainingJob> RetrainModelAsync(string modelName, object parameters);
    Task<AIModel> UpdateModelSettingsAsync(string modelName, object settings);
    Task<bool> DeleteModelAsync(string modelName);
    Task<string> ExportModelAsync(string modelName);
    Task<TrainingHistory?> GetTrainingDetailsAsync(string modelName, DateTime date);
    Task<string> GenerateTrainingReportAsync(string modelName, DateTime date);
}

/// <summary>
/// Orchestrator service for AI management operations
/// </summary>
public class AIManagementService : IAIManagementService
{
    private readonly IModelManagementService _modelManagementService;
    private readonly ITrainingManagementService _trainingManagementService;
    private readonly ITrainingHistoryService _trainingHistoryService;
    private readonly ILogger<AIManagementService> _logger;

    public AIManagementService(
        IModelManagementService modelManagementService,
        ITrainingManagementService trainingManagementService,
        ITrainingHistoryService trainingHistoryService,
        ILogger<AIManagementService> logger)
    {
        _modelManagementService = modelManagementService;
        _trainingManagementService = trainingManagementService;
        _trainingHistoryService = trainingHistoryService;
        _logger = logger;
    }

    #region Model Management - Delegate to ModelManagementService

    public async Task<IEnumerable<AIModel>> GetAllModelsAsync()
        => await _modelManagementService.GetAllModelsAsync();

    public async Task<AIModel?> GetModelByIdAsync(Guid id)
        => await _modelManagementService.GetModelByIdAsync(id);

    public async Task<AIModel> UpdateModelSettingsAsync(string modelName, object settings)
        => await _modelManagementService.UpdateModelSettingsAsync(modelName, settings);

    public async Task<bool> DeleteModelAsync(string modelName)
        => await _modelManagementService.DeleteModelAsync(modelName);

    public async Task<string> ExportModelAsync(string modelName)
        => await _modelManagementService.ExportModelAsync(modelName);

    #endregion

    #region Training Management - Delegate to TrainingManagementService

    public async Task<IEnumerable<TrainingJob>> GetTrainingQueueAsync()
        => await _trainingManagementService.GetTrainingQueueAsync();

    public async Task<TrainingJob> StartTrainingAsync(Guid modelId, object parameters)
        => await _trainingManagementService.StartTrainingAsync(modelId, parameters);

    public async Task<TrainingJob> RetrainModelAsync(string modelName, object parameters)
        => await _trainingManagementService.RetrainModelAsync(modelName, parameters);

    #endregion

    #region Training History - Delegate to TrainingHistoryService

    public async Task<IEnumerable<TrainingHistory>> GetRecentTrainingHistoryAsync(int count = 10)
        => await _trainingHistoryService.GetRecentTrainingHistoryAsync(count);

    public async Task<TrainingHistory?> GetTrainingDetailsAsync(string modelName, DateTime date)
        => await _trainingHistoryService.GetTrainingDetailsAsync(modelName, date);

    public async Task<string> GenerateTrainingReportAsync(string modelName, DateTime date)
        => await _trainingHistoryService.GenerateTrainingReportAsync(modelName, date);

    #endregion
}


