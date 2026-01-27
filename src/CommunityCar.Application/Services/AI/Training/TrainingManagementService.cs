using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Services.AI.ModelManagement;
using CommunityCar.Domain.Entities.AI;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CommunityCar.Application.Services.AI.Training;

/// <summary>
/// Service for AI training management operations
/// </summary>
public class TrainingManagementService : ITrainingManagementService
{
    private readonly ITrainingJobRepository _jobRepository;
    private readonly IModelManagementService _modelManagementService;
    private readonly ILogger<TrainingManagementService> _logger;

    public TrainingManagementService(
        ITrainingJobRepository jobRepository,
        IModelManagementService modelManagementService,
        ILogger<TrainingManagementService> logger)
    {
        _jobRepository = jobRepository;
        _modelManagementService = modelManagementService;
        _logger = logger;
    }

    public async Task<IEnumerable<TrainingJob>> GetTrainingQueueAsync()
    {
        return await _jobRepository.GetQueuedJobsAsync();
    }

    public async Task<TrainingJob> StartTrainingAsync(Guid modelId, object parameters)
    {
        var model = await _modelManagementService.GetModelByIdAsync(modelId);
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
        var model = await _modelManagementService.GetModelByNameAsync(modelName);
        if (model == null)
            throw new ArgumentException($"Model '{modelName}' not found");

        return await StartTrainingAsync(model.Id, parameters);
    }
}


