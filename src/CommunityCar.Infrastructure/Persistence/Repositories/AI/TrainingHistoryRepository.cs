using Microsoft.EntityFrameworkCore;
using CommunityCar.Application.Common.Interfaces.Repositories.AI;
using CommunityCar.Domain.Entities.AI;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;

namespace CommunityCar.Infrastructure.Persistence.Repositories.AI;

public class TrainingHistoryRepository : BaseRepository<TrainingHistory>, ITrainingHistoryRepository
{
    public TrainingHistoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TrainingHistory>> GetHistoryByModelIdAsync(Guid modelId)
    {
        return await Context.TrainingHistories
            .Where(h => h.AIModelId == modelId && !h.IsDeleted)
            .OrderByDescending(h => h.TrainingDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<TrainingHistory>> GetHistoryByJobIdAsync(Guid jobId)
    {
        return await Context.TrainingHistories
            .Where(h => h.TrainingJobId == jobId && !h.IsDeleted)
            .OrderByDescending(h => h.TrainingDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<TrainingHistory>> GetRecentHistoryAsync(int count = 10)
    {
        return await Context.TrainingHistories
            .Where(h => !h.IsDeleted)
            .OrderByDescending(h => h.TrainingDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<TrainingHistory>> GetSuccessfulTrainingsAsync(Guid modelId)
    {
        return await Context.TrainingHistories
            .Where(h => h.AIModelId == modelId && h.IsSuccessful && !h.IsDeleted)
            .OrderByDescending(h => h.TrainingDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<TrainingHistory>> GetFailedTrainingsAsync(Guid modelId)
    {
        return await Context.TrainingHistories
            .Where(h => h.AIModelId == modelId && !h.IsSuccessful && !h.IsDeleted)
            .OrderByDescending(h => h.TrainingDate)
            .ToListAsync();
    }

    public async Task<TrainingHistory?> GetLatestTrainingAsync(Guid modelId)
    {
        return await Context.TrainingHistories
            .Where(h => h.AIModelId == modelId && !h.IsDeleted)
            .OrderByDescending(h => h.TrainingDate)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> DeleteOldHistoryAsync(DateTime cutoffDate)
    {
        var oldHistory = await Context.TrainingHistories
            .Where(h => h.TrainingDate < cutoffDate && !h.IsDeleted)
            .ToListAsync();

        if (oldHistory.Any())
        {
            foreach (var history in oldHistory)
            {
                history.SoftDelete("System");
            }
            Context.TrainingHistories.UpdateRange(oldHistory);
            return true;
        }

        return false;
    }
}