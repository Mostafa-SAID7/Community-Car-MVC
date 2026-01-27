using Microsoft.EntityFrameworkCore;
using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Domain.Entities.AI;
using CommunityCar.Infrastructure.Persistence.Data;

namespace CommunityCar.Infrastructure.Persistence.Repositories.AI;

public class AIModelRepository : IAIModelRepository
{
    private readonly ApplicationDbContext _context;

    public AIModelRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AIModel>> GetAllModelsAsync()
    {
        return await _context.AIModels
            .Include(m => m.TrainingJobs)
            .Include(m => m.TrainingHistories)
            .Where(m => !m.IsDeleted)
            .OrderBy(m => m.Name)
            .ToListAsync();
    }

    public async Task<AIModel?> GetModelByIdAsync(Guid id)
    {
        return await _context.AIModels
            .Include(m => m.TrainingJobs)
            .Include(m => m.TrainingHistories)
            .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);
    }

    public async Task<AIModel?> GetModelByNameAsync(string name)
    {
        return await _context.AIModels
            .Include(m => m.TrainingJobs)
            .Include(m => m.TrainingHistories)
            .FirstOrDefaultAsync(m => m.Name == name && !m.IsDeleted);
    }

    public async Task<IEnumerable<AIModel>> GetActiveModelsAsync()
    {
        return await _context.AIModels
            .Include(m => m.TrainingJobs)
            .Include(m => m.TrainingHistories)
            .Where(m => m.IsActive && !m.IsDeleted)
            .OrderBy(m => m.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<AIModel>> GetModelsByTypeAsync(AIModelType type)
    {
        return await _context.AIModels
            .Include(m => m.TrainingJobs)
            .Include(m => m.TrainingHistories)
            .Where(m => m.Type == type && !m.IsDeleted)
            .OrderBy(m => m.Name)
            .ToListAsync();
    }

    public async Task<AIModel> CreateModelAsync(AIModel model)
    {
        _context.AIModels.Add(model);
        await _context.SaveChangesAsync();
        return model;
    }

    public async Task<AIModel> UpdateModelAsync(AIModel model)
    {
        _context.AIModels.Update(model);
        await _context.SaveChangesAsync();
        return model;
    }

    public async Task<bool> DeleteModelAsync(Guid id)
    {
        var model = await _context.AIModels.FindAsync(id);
        if (model == null) return false;

        model.SoftDelete("System");
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ModelExistsAsync(string name)
    {
        return await _context.AIModels
            .AnyAsync(m => m.Name == name && !m.IsDeleted);
    }
}

public class TrainingJobRepository : ITrainingJobRepository
{
    private readonly ApplicationDbContext _context;

    public TrainingJobRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TrainingJob>> GetAllJobsAsync()
    {
        return await _context.TrainingJobs
            .Include(j => j.AIModel)
            .Where(j => !j.IsDeleted)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TrainingJob>> GetQueuedJobsAsync()
    {
        return await _context.TrainingJobs
            .Include(j => j.AIModel)
            .Where(j => j.Status == TrainingJobStatus.Queued || j.Status == TrainingJobStatus.InProgress)
            .Where(j => !j.IsDeleted)
            .OrderBy(j => j.Priority)
            .ThenBy(j => j.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TrainingJob>> GetJobsByModelIdAsync(Guid modelId)
    {
        return await _context.TrainingJobs
            .Include(j => j.AIModel)
            .Where(j => j.AIModelId == modelId && !j.IsDeleted)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();
    }

    public async Task<TrainingJob?> GetJobByIdAsync(Guid id)
    {
        return await _context.TrainingJobs
            .Include(j => j.AIModel)
            .FirstOrDefaultAsync(j => j.Id == id && !j.IsDeleted);
    }

    public async Task<TrainingJob> CreateJobAsync(TrainingJob job)
    {
        _context.TrainingJobs.Add(job);
        await _context.SaveChangesAsync();
        return job;
    }

    public async Task<TrainingJob> UpdateJobAsync(TrainingJob job)
    {
        _context.TrainingJobs.Update(job);
        await _context.SaveChangesAsync();
        return job;
    }

    public async Task<bool> DeleteJobAsync(Guid id)
    {
        var job = await _context.TrainingJobs.FindAsync(id);
        if (job == null) return false;

        job.SoftDelete("System");
        await _context.SaveChangesAsync();
        return true;
    }
}

public class TrainingHistoryRepository : ITrainingHistoryRepository
{
    private readonly ApplicationDbContext _context;

    public TrainingHistoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TrainingHistory>> GetHistoryByModelIdAsync(Guid modelId)
    {
        return await _context.TrainingHistories
            .Include(h => h.AIModel)
            .Where(h => h.AIModelId == modelId && !h.IsDeleted)
            .OrderByDescending(h => h.TrainingDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<TrainingHistory>> GetRecentHistoryAsync(int count = 10)
    {
        return await _context.TrainingHistories
            .Include(h => h.AIModel)
            .Where(h => !h.IsDeleted)
            .OrderByDescending(h => h.TrainingDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task<TrainingHistory?> GetHistoryByIdAsync(Guid id)
    {
        return await _context.TrainingHistories
            .Include(h => h.AIModel)
            .FirstOrDefaultAsync(h => h.Id == id && !h.IsDeleted);
    }

    public async Task<TrainingHistory> CreateHistoryAsync(TrainingHistory history)
    {
        _context.TrainingHistories.Add(history);
        await _context.SaveChangesAsync();
        return history;
    }

    public async Task<TrainingHistory> UpdateHistoryAsync(TrainingHistory history)
    {
        _context.TrainingHistories.Update(history);
        await _context.SaveChangesAsync();
        return history;
    }

    public async Task<bool> DeleteHistoryAsync(Guid id)
    {
        var history = await _context.TrainingHistories.FindAsync(id);
        if (history == null) return false;

        history.SoftDelete("System");
        await _context.SaveChangesAsync();
        return true;
    }
}
