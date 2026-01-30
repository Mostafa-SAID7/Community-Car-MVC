using Microsoft.EntityFrameworkCore;
using CommunityCar.Application.Common.Interfaces.Repositories.AI;
using CommunityCar.Domain.Entities.AI;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;

namespace CommunityCar.Infrastructure.Persistence.Repositories.AI;

public class AIModelRepository : BaseRepository<AIModel>, IAIModelRepository
{
    public AIModelRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<AIModel>> GetActiveModelsAsync()
    {
        return await Context.AIModels
            .Where(m => m.IsActive && !m.IsDeleted)
            .OrderBy(m => m.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<AIModel>> GetModelsByTypeAsync(AIModelType modelType)
    {
        return await Context.AIModels
            .Where(m => m.Type == modelType && !m.IsDeleted)
            .OrderBy(m => m.Name)
            .ToListAsync();
    }

    public async Task<AIModel?> GetModelByNameAsync(string name)
    {
        return await Context.AIModels
            .FirstOrDefaultAsync(m => m.Name == name && !m.IsDeleted);
    }

    public async Task<bool> ModelExistsAsync(string name)
    {
        return await Context.AIModels
            .AnyAsync(m => m.Name == name && !m.IsDeleted);
    }

    public async Task<IEnumerable<AIModel>> GetModelsByUserAsync(Guid userId)
    {
        return await Context.AIModels
            .Where(m => m.CreatedBy == userId.ToString() && !m.IsDeleted)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> ActivateModelAsync(Guid id)
    {
        var model = await GetByIdAsync(id);
        if (model == null) return false;

        model.Activate();
        await UpdateAsync(model);
        return true;
    }

    public async Task<bool> DeactivateModelAsync(Guid id)
    {
        var model = await GetByIdAsync(id);
        if (model == null) return false;

        model.Deactivate();
        await UpdateAsync(model);
        return true;
    }
}


