using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Domain.Entities.AI;
using Microsoft.Extensions.Logging;
using System.Text.Json;

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

public class AIManagementService : IAIManagementService
{
    private readonly IAIModelRepository _modelRepository;
    private readonly ITrainingJobRepository _jobRepository;
    private readonly ITrainingHistoryRepository _historyRepository;
    private readonly ILogger<AIManagementService> _logger;

    public AIManagementService(
        IAIModelRepository modelRepository,
        ITrainingJobRepository jobRepository,
        ITrainingHistoryRepository historyRepository,
        ILogger<AIManagementService> logger)
    {
        _modelRepository = modelRepository;
        _jobRepository = jobRepository;
        _historyRepository = historyRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<AIModel>> GetAllModelsAsync()
    {
        return await _modelRepository.GetAllModelsAsync();
    }

    public async Task<IEnumerable<TrainingJob>> GetTrainingQueueAsync()
    {
        return await _jobRepository.GetQueuedJobsAsync();
    }

    public async Task<IEnumerable<TrainingHistory>> GetRecentTrainingHistoryAsync(int count = 10)
    {
        return await _historyRepository.GetRecentHistoryAsync(count);
    }

    public async Task<AIModel?> GetModelByIdAsync(Guid id)
    {
        return await _modelRepository.GetModelByIdAsync(id);
    }

    public async Task<TrainingJob> StartTrainingAsync(Guid modelId, object parameters)
    {
        var model = await _modelRepository.GetModelByIdAsync(modelId);
        if (model == null)
            throw new ArgumentException($"Model with ID {modelId} not found");

        var job = new TrainingJob
        {
            AIModelId = modelId,
            JobName = $"{model.Name} Training - {DateTime.UtcNow:yyyy-MM-dd HH:mm}",
            Status = TrainingJobStatus.Queued,
            StartedAt = DateTime.UtcNow,
            EstimatedDuration = TimeSpan.FromHours(2), // Default estimate
            Parameters = JsonSerializer.Serialize(parameters),
            Priority = 1
        };

        return await _jobRepository.CreateJobAsync(job);
    }

    public async Task<TrainingJob> RetrainModelAsync(string modelName, object parameters)
    {
        var model = await _modelRepository.GetModelByNameAsync(modelName);
        if (model == null)
            throw new ArgumentException($"Model '{modelName}' not found");

        return await StartTrainingAsync(model.Id, parameters);
    }

    public async Task<AIModel> UpdateModelSettingsAsync(string modelName, object settings)
    {
        var model = await _modelRepository.GetModelByNameAsync(modelName);
        if (model == null)
            throw new ArgumentException($"Model '{modelName}' not found");

        model.Configuration = JsonSerializer.Serialize(settings);

        return await _modelRepository.UpdateModelAsync(model);
    }

    public async Task<bool> DeleteModelAsync(string modelName)
    {
        var model = await _modelRepository.GetModelByNameAsync(modelName);
        if (model == null)
            return false;

        return await _modelRepository.DeleteModelAsync(model.Id);
    }

    public async Task<string> ExportModelAsync(string modelName)
    {
        var model = await _modelRepository.GetModelByNameAsync(modelName);
        if (model == null)
            throw new ArgumentException($"Model '{modelName}' not found");

        // In a real implementation, this would create a downloadable package
        var exportPath = $"/exports/{modelName.ToLower().Replace(" ", "_")}_model_{DateTime.UtcNow:yyyyMMdd}.zip";
        
        _logger.LogInformation("Exporting model {ModelName} to {ExportPath}", modelName, exportPath);
        
        return exportPath;
    }

    public async Task<TrainingHistory?> GetTrainingDetailsAsync(string modelName, DateTime date)
    {
        var model = await _modelRepository.GetModelByNameAsync(modelName);
        if (model == null)
            return null;

        var histories = await _historyRepository.GetHistoryByModelIdAsync(model.Id);
        return histories.FirstOrDefault(h => h.TrainingDate.Date == date.Date);
    }

    public async Task<string> GenerateTrainingReportAsync(string modelName, DateTime date)
    {
        var history = await GetTrainingDetailsAsync(modelName, date);
        if (history == null)
            throw new ArgumentException($"No training history found for {modelName} on {date:yyyy-MM-dd}");

        // In a real implementation, this would generate a PDF report
        var reportPath = $"/reports/{modelName.ToLower().Replace(" ", "_")}_training_report_{date:yyyyMMdd}.pdf";
        
        _logger.LogInformation("Generating training report for {ModelName} on {Date}", modelName, date);
        
        return reportPath;
    }
}