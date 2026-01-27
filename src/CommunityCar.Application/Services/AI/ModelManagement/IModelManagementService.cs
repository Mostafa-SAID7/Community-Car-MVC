using CommunityCar.Domain.Entities.AI;

namespace CommunityCar.Application.Services.AI.ModelManagement;

/// <summary>
/// Interface for AI model management operations
/// </summary>
public interface IModelManagementService
{
    Task<IEnumerable<AIModel>> GetAllModelsAsync();
    Task<AIModel?> GetModelByIdAsync(Guid id);
    Task<AIModel?> GetModelByNameAsync(string name);
    Task<AIModel> UpdateModelSettingsAsync(string modelName, object settings);
    Task<bool> DeleteModelAsync(string modelName);
    Task<string> ExportModelAsync(string modelName);
}


