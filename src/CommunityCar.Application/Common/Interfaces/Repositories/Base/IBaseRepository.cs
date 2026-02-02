using CommunityCar.Domain.Base;
using System.Linq.Expressions;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Base;

public interface IBaseRepository<T> where T : class, IBaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task DeleteRangeAsync(IEnumerable<T> entities);
    
    // Soft Delete Methods
    Task<T?> GetByIdIncludeDeletedAsync(Guid id);
    Task<IEnumerable<T>> GetAllIncludeDeletedAsync();
    Task<IEnumerable<T>> GetDeletedOnlyAsync();
    Task<IEnumerable<T>> FindIncludeDeletedAsync(Expression<Func<T, bool>> predicate);
    Task<bool> SoftDeleteAsync(Guid id, string? deletedBy = null);
    Task<bool> SoftDeleteAsync(T entity, string? deletedBy = null);
    Task<int> SoftDeleteRangeAsync(IEnumerable<Guid> ids, string? deletedBy = null);
    Task<int> SoftDeleteRangeAsync(IEnumerable<T> entities, string? deletedBy = null);
    Task<bool> RestoreAsync(Guid id, string? restoredBy = null);
    Task<bool> RestoreAsync(T entity, string? restoredBy = null);
    Task<int> RestoreRangeAsync(IEnumerable<Guid> ids, string? restoredBy = null);
    Task<int> RestoreRangeAsync(IEnumerable<T> entities, string? restoredBy = null);
    Task<bool> PermanentDeleteAsync(Guid id);
    Task<bool> PermanentDeleteAsync(T entity);
    Task<int> PermanentDeleteRangeAsync(IEnumerable<Guid> ids);
    Task<int> PermanentDeleteRangeAsync(IEnumerable<T> entities);
}


