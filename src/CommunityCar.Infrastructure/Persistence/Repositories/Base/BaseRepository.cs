using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Base;
using CommunityCar.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Base;

public class BaseRepository<T> : IBaseRepository<T> where T : class, IBaseEntity
{
    protected readonly ApplicationDbContext Context;
    protected readonly DbSet<T> DbSet;

    public BaseRepository(ApplicationDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await DbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await DbSet.ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await DbSet.Where(predicate).ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await DbSet.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await DbSet.AddRangeAsync(entities);
    }

    public Task UpdateAsync(T entity)
    {
        DbSet.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity)
    {
        DbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public Task DeleteRangeAsync(IEnumerable<T> entities)
    {
        DbSet.RemoveRange(entities);
        return Task.CompletedTask;
    }

    // Soft Delete Methods - Basic implementations
    public async Task<T?> GetByIdIncludeDeletedAsync(Guid id)
    {
        return await DbSet.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<T>> GetAllIncludeDeletedAsync()
    {
        return await DbSet.IgnoreQueryFilters().ToListAsync();
    }

    public async Task<IEnumerable<T>> GetDeletedOnlyAsync()
    {
        return await DbSet.IgnoreQueryFilters().Where(e => e.IsDeleted).ToListAsync();
    }

    public async Task<IEnumerable<T>> FindIncludeDeletedAsync(Expression<Func<T, bool>> predicate)
    {
        return await DbSet.IgnoreQueryFilters().Where(predicate).ToListAsync();
    }

    public async Task<bool> SoftDeleteAsync(Guid id, string? deletedBy = null)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null) return false;
        return await SoftDeleteAsync(entity, deletedBy);
    }

    public async Task<bool> SoftDeleteAsync(T entity, string? deletedBy = null)
    {
        if (entity is ISoftDeletable softDeletable)
        {
            softDeletable.SoftDelete(deletedBy);
        }
        else
        {
            // Fallback for entities that don't implement ISoftDeletable but have the properties
            var isDeletedProperty = typeof(T).GetProperty("IsDeleted");
            var deletedAtProperty = typeof(T).GetProperty("DeletedAt");
            var deletedByProperty = typeof(T).GetProperty("DeletedBy");
            
            if (isDeletedProperty != null && isDeletedProperty.CanWrite)
                isDeletedProperty.SetValue(entity, true);
            if (deletedAtProperty != null && deletedAtProperty.CanWrite)
                deletedAtProperty.SetValue(entity, DateTime.UtcNow);
            if (deletedByProperty != null && deletedByProperty.CanWrite)
                deletedByProperty.SetValue(entity, deletedBy);
        }
        
        await UpdateAsync(entity);
        return true;
    }

    public async Task<int> SoftDeleteRangeAsync(IEnumerable<Guid> ids, string? deletedBy = null)
    {
        var entities = await DbSet.Where(e => ids.Contains(e.Id)).ToListAsync();
        return await SoftDeleteRangeAsync(entities, deletedBy);
    }

    public async Task<int> SoftDeleteRangeAsync(IEnumerable<T> entities, string? deletedBy = null)
    {
        var count = 0;
        foreach (var entity in entities)
        {
            if (await SoftDeleteAsync(entity, deletedBy))
                count++;
        }
        return count;
    }

    public async Task<bool> RestoreAsync(Guid id, string? restoredBy = null)
    {
        var entity = await GetByIdIncludeDeletedAsync(id);
        if (entity == null || !entity.IsDeleted) return false;
        return await RestoreAsync(entity, restoredBy);
    }

    public async Task<bool> RestoreAsync(T entity, string? restoredBy = null)
    {
        if (entity is ISoftDeletable softDeletable)
        {
            softDeletable.Restore(restoredBy);
        }
        else
        {
            // Fallback for entities that don't implement ISoftDeletable but have the properties
            var isDeletedProperty = typeof(T).GetProperty("IsDeleted");
            var deletedAtProperty = typeof(T).GetProperty("DeletedAt");
            var deletedByProperty = typeof(T).GetProperty("DeletedBy");
            
            if (isDeletedProperty != null && isDeletedProperty.CanWrite)
                isDeletedProperty.SetValue(entity, false);
            if (deletedAtProperty != null && deletedAtProperty.CanWrite)
                deletedAtProperty.SetValue(entity, null);
            if (deletedByProperty != null && deletedByProperty.CanWrite)
                deletedByProperty.SetValue(entity, null);
        }
        
        await UpdateAsync(entity);
        return true;
    }

    public async Task<int> RestoreRangeAsync(IEnumerable<Guid> ids, string? restoredBy = null)
    {
        var entities = await DbSet.IgnoreQueryFilters().Where(e => ids.Contains(e.Id) && e.IsDeleted).ToListAsync();
        return await RestoreRangeAsync(entities, restoredBy);
    }

    public async Task<int> RestoreRangeAsync(IEnumerable<T> entities, string? restoredBy = null)
    {
        var count = 0;
        foreach (var entity in entities)
        {
            if (await RestoreAsync(entity, restoredBy))
                count++;
        }
        return count;
    }

    public async Task<bool> PermanentDeleteAsync(Guid id)
    {
        var entity = await GetByIdIncludeDeletedAsync(id);
        if (entity == null) return false;
        return await PermanentDeleteAsync(entity);
    }

    public async Task<bool> PermanentDeleteAsync(T entity)
    {
        DbSet.Remove(entity);
        return true;
    }

    public async Task<int> PermanentDeleteRangeAsync(IEnumerable<Guid> ids)
    {
        var entities = await DbSet.IgnoreQueryFilters().Where(e => ids.Contains(e.Id)).ToListAsync();
        return await PermanentDeleteRangeAsync(entities);
    }

    public async Task<int> PermanentDeleteRangeAsync(IEnumerable<T> entities)
    {
        DbSet.RemoveRange(entities);
        return entities.Count();
    }
}
