using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using CommunityCar.Application.Common.Models;

namespace CommunityCar.Application.Common.Extensions;

/// <summary>
/// Extensions for IQueryable to support common data operations
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Applies pagination to a queryable
    /// </summary>
    public static async Task<PaginatedResult<T>> ToPaginatedListAsync<T>(
        this IQueryable<T> source,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var count = await source.CountAsync(cancellationToken);
        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<T>(items, count, pageNumber, pageSize);
    }

    /// <summary>
    /// Applies conditional where clause
    /// </summary>
    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> source,
        bool condition,
        Expression<Func<T, bool>> predicate)
    {
        return condition ? source.Where(predicate) : source;
    }

    /// <summary>
    /// Applies search filter across multiple string properties
    /// </summary>
    public static IQueryable<T> Search<T>(
        this IQueryable<T> source,
        string? searchTerm,
        params Expression<Func<T, string>>[] properties)
    {
        if (string.IsNullOrWhiteSpace(searchTerm) || !properties.Any())
            return source;

        var parameter = Expression.Parameter(typeof(T), "x");
        Expression? combinedExpression = null;

        foreach (var property in properties)
        {
            var propertyAccess = Expression.Invoke(property, parameter);
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
            var searchExpression = Expression.Call(
                propertyAccess,
                containsMethod,
                Expression.Constant(searchTerm, typeof(string)));

            combinedExpression = combinedExpression == null
                ? searchExpression
                : Expression.OrElse(combinedExpression, searchExpression);
        }

        if (combinedExpression != null)
        {
            var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
            source = source.Where(lambda);
        }

        return source;
    }

    /// <summary>
    /// Orders by multiple properties dynamically
    /// </summary>
    public static IQueryable<T> OrderByDynamic<T>(
        this IQueryable<T> source,
        string? sortBy,
        bool descending = false)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return source;

        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, sortBy);
        var lambda = Expression.Lambda(property, parameter);

        var methodName = descending ? "OrderByDescending" : "OrderBy";
        var resultExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            new[] { typeof(T), property.Type },
            source.Expression,
            Expression.Quote(lambda));

        return source.Provider.CreateQuery<T>(resultExpression);
    }

    /// <summary>
    /// Includes related entities conditionally
    /// </summary>
    public static IQueryable<T> IncludeIf<T, TProperty>(
        this IQueryable<T> source,
        bool condition,
        Expression<Func<T, TProperty>> navigationPropertyPath)
        where T : class
    {
        return condition ? source.Include(navigationPropertyPath) : source;
    }

    /// <summary>
    /// Filters by date range
    /// </summary>
    public static IQueryable<T> FilterByDateRange<T>(
        this IQueryable<T> source,
        Expression<Func<T, DateTime>> dateSelector,
        DateTime? startDate,
        DateTime? endDate)
    {
        if (startDate.HasValue)
        {
            var startPredicate = Expression.Lambda<Func<T, bool>>(
                Expression.GreaterThanOrEqual(
                    Expression.Invoke(dateSelector, dateSelector.Parameters[0]),
                    Expression.Constant(startDate.Value)),
                dateSelector.Parameters[0]);
            source = source.Where(startPredicate);
        }

        if (endDate.HasValue)
        {
            var endPredicate = Expression.Lambda<Func<T, bool>>(
                Expression.LessThanOrEqual(
                    Expression.Invoke(dateSelector, dateSelector.Parameters[0]),
                    Expression.Constant(endDate.Value)),
                dateSelector.Parameters[0]);
            source = source.Where(endPredicate);
        }

        return source;
    }

    /// <summary>
    /// Applies soft delete filter
    /// </summary>
    public static IQueryable<T> WhereNotDeleted<T>(this IQueryable<T> source)
        where T : class
    {
        var entityType = typeof(T);
        var isDeletedProperty = entityType.GetProperty("IsDeleted");
        
        if (isDeletedProperty == null || isDeletedProperty.PropertyType != typeof(bool))
            return source;

        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, "IsDeleted");
        var condition = Expression.Equal(property, Expression.Constant(false));
        var lambda = Expression.Lambda<Func<T, bool>>(condition, parameter);

        return source.Where(lambda);
    }
}

/// <summary>
/// Paginated result wrapper
/// </summary>
public class PaginatedResult<T>
{
    public List<T> Items { get; }
    public int TotalCount { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public PaginatedResult(List<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}