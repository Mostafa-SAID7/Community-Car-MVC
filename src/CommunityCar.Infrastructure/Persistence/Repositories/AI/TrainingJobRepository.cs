using Microsoft.EntityFrameworkCore;
using CommunityCar.Application.Common.Interfaces.Repositories.AI;
using CommunityCar.Domain.Entities.AI;
using CommunityCar.Domain.Enums.AI;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;

namespace CommunityCar.Infrastructure.Persistence.Repositories.AI;

public class TrainingJobRepository : BaseRepository<TrainingJob>, ITrainingJobRepository
{
    public TrainingJobRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TrainingJob>> GetQueuedJobsAsync()
    {
        return await Context.TrainingJobs
            .Where(j => j.Status == TrainingJobStatus.Queued && !j.IsDeleted)
            .OrderBy(j => j.Priority)
            .ThenBy(j => j.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TrainingJob>> GetRunningJobsAsync()
    {
        return await Context.TrainingJobs
            .Where(j => j.Status == TrainingJobStatus.InProgress && !j.IsDeleted)
            .OrderBy(j => j.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TrainingJob>> GetCompletedJobsAsync()
    {
        return await Context.TrainingJobs
            .Where(j => (j.Status == TrainingJobStatus.Completed || j.Status == TrainingJobStatus.Failed) && !j.IsDeleted)
            .OrderByDescending(j => j.CompletedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TrainingJob>> GetJobsByModelIdAsync(Guid modelId)
    {
        return await Context.TrainingJobs
            .Where(j => j.AIModelId == modelId && !j.IsDeleted)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TrainingJob>> GetJobsByUserAsync(Guid userId)
    {
        return await Context.TrainingJobs
            .Where(j => j.CreatedBy == userId.ToString() && !j.IsDeleted)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> StartJobAsync(Guid jobId)
    {
        var job = await GetByIdAsync(jobId);
        if (job == null || job.Status != TrainingJobStatus.Queued) return false;

        job.Status = TrainingJobStatus.InProgress;
        job.StartedAt = DateTime.UtcNow;
        await UpdateAsync(job);
        return true;
    }

    public async Task<bool> CompleteJobAsync(Guid jobId, bool success, string? errorMessage = null)
    {
        var job = await GetByIdAsync(jobId);
        if (job == null || job.Status != TrainingJobStatus.InProgress) return false;

        if (success)
        {
            job.Status = TrainingJobStatus.Completed;
            job.CompletedAt = DateTime.UtcNow;
        }
        else
        {
            job.Status = TrainingJobStatus.Failed;
            job.CompletedAt = DateTime.UtcNow;
            job.ErrorMessage = errorMessage ?? "Training failed";
        }

        await UpdateAsync(job);
        return true;
    }

    public async Task<bool> CancelJobAsync(Guid jobId)
    {
        var job = await GetByIdAsync(jobId);
        if (job == null || (job.Status != TrainingJobStatus.Queued && job.Status != TrainingJobStatus.InProgress)) return false;

        job.Status = TrainingJobStatus.Cancelled;
        job.CompletedAt = DateTime.UtcNow;
        await UpdateAsync(job);
        return true;
    }

    public async Task<int> GetQueuePositionAsync(Guid jobId)
    {
        var job = await GetByIdAsync(jobId);
        if (job == null || job.Status != TrainingJobStatus.Queued) return -1;

        return await Context.TrainingJobs
            .Where(j => j.Status == TrainingJobStatus.Queued && !j.IsDeleted)
            .Where(j => j.Priority < job.Priority || 
                       (j.Priority == job.Priority && j.CreatedAt < job.CreatedAt))
            .CountAsync() + 1;
    }
}