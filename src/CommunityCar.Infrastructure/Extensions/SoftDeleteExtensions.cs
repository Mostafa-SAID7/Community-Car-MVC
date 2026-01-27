using CommunityCar.Domain.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;
using System.Reflection;

namespace CommunityCar.Infrastructure.Extensions;

public static class SoftDeleteExtensions
{
    public static void ConfigureSoftDeleteFilter(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(SoftDeleteExtensions)
                    .GetMethod(nameof(GetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(entityType.ClrType);
                
                var filter = method.Invoke(null, Array.Empty<object>());
                entityType.SetQueryFilter((LambdaExpression)filter!);
            }
        }
    }

    private static LambdaExpression GetSoftDeleteFilter<TEntity>()
        where TEntity : class, ISoftDeletable
    {
        Expression<Func<TEntity, bool>> filter = x => !x.IsDeleted;
        return filter;
    }

    public static IQueryable<T> IncludeDeleted<T>(this IQueryable<T> query)
        where T : class, ISoftDeletable
    {
        return query.IgnoreQueryFilters();
    }

    public static IQueryable<T> OnlyDeleted<T>(this IQueryable<T> query)
        where T : class, ISoftDeletable
    {
        return query.IgnoreQueryFilters().Where(x => x.IsDeleted);
    }

    public static async Task<int> SoftDeleteAsync<T>(this DbSet<T> dbSet, Guid id, string deletedBy)
        where T : class, ISoftDeletable, IBaseEntity
    {
        var entity = await dbSet.FindAsync(id);
        if (entity == null) return 0;

        entity.SoftDelete(deletedBy);
        return 1;
    }

    public static async Task<int> RestoreAsync<T>(this DbSet<T> dbSet, Guid id, string restoredBy)
        where T : class, ISoftDeletable, IBaseEntity
    {
        var entity = await dbSet.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return 0;

        entity.Restore(restoredBy);
        return 1;
    }
}
