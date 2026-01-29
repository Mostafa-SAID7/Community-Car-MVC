using CommunityCar.Application.Common.Interfaces.Repositories.AI;
using CommunityCar.Application.Services.AI.ModelManagement;
using CommunityCar.Domain.Entities.AI;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.AI.History;

/// <summary>
/// Service for training history operations
/// </summary>
public class TrainingHistoryService : ITrainingHistoryService
{
    private readonly ITrainingHistoryRepository _historyRepository;
    private readonly IModelManagementService _modelManagementService;
    private readonly ILogger<TrainingHistoryService> _logger;

    public TrainingHistoryService(
        ITrainingHistoryRepository historyRepository,
        IModelManagementService modelManagementService,
        ILogger<TrainingHistoryService> logger)
    {
        _historyRepository = historyRepository;
        _modelManagementService = modelManagementService;
        _logger = logger;
    }

    public async Task<IEnumerable<TrainingHistory>> GetRecentTrainingHistoryAsync(int count = 10)
    {
        return await _historyRepository.GetRecentHistoryAsync(count);
    }

    public async Task<TrainingHistory?> GetTrainingDetailsAsync(string modelName, DateTime date)
    {
        var model = await _modelManagementService.GetModelByNameAsync(modelName);
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


