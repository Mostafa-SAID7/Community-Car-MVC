using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Domain.Entities.AI;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CommunityCar.Application.Services.AI.ModelManagement;

/// <summary>
/// Service for AI model management operations
/// </summary>
public class ModelManagementService : IModelManagementService
{
    private readonly IAIModelRepository _modelRepository;
    private readonly ILogger<ModelManagementService> _logger;

    public ModelManagementService(
        IAIModelRepository modelRepository,
        ILogger<ModelManagementService> logger)
    {
        _modelRepository = modelRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<AIModel>> GetAllModelsAsync()
    {
        return await _modelRepository.GetAllModelsAsync();
    }

    public async Task<AIModel?> GetModelByIdAsync(Guid id)
    {
        return await _modelRepository.GetModelByIdAsync(id);
    }

    public async Task<AIModel?> GetModelByNameAsync(string name)
    {
        return await _modelRepository.GetModelByNameAsync(name);
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
}


