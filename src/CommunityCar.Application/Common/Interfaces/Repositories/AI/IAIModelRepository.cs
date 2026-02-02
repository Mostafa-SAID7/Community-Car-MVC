using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.AI;
using CommunityCar.Domain.Enums.AI;

namespace CommunityCar.Application.Common.Interfaces.Repositories.AI;

public interface IAIModelRepository : IBaseRepository<AIModel>
{
    Task<IEnumerable<AIModel>> GetActiveModelsAsync();
    Task<IEnumerable<AIModel>> GetModelsByTypeAsync(AIModelType modelType);
    Task<AIModel?> GetModelByNameAsync(string name);
    Task<bool> ModelExistsAsync(string name);
    Task<IEnumerable<AIModel>> GetModelsByUserAsync(Guid userId);
    Task<bool> ActivateModelAsync(Guid id);
    Task<bool> DeactivateModelAsync(Guid id);
}