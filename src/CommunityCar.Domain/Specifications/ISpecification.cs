using System.Linq.Expressions;

namespace CommunityCar.Domain.Specifications;

/// <summary>
/// Base specification interface for implementing the Specification pattern
/// </summary>
/// <typeparam name="T">The entity type</typeparam>
public interface ISpecification<T>
{
    /// <summary>
    /// Gets the criteria expression for the specification
    /// </summary>
    Expression<Func<T, bool>> Criteria { get; }
    
    /// <summary>
    /// Gets the list of include expressions for eager loading
    /// </summary>
    List<Expression<Func<T, object>>> Includes { get; }
    
    /// <summary>
    /// Gets the list of include string expressions for eager loading
    /// </summary>
    List<string> IncludeStrings { get; }
    
    /// <summary>
    /// Gets the order by expression
    /// </summary>
    Expression<Func<T, object>>? OrderBy { get; }
    
    /// <summary>
    /// Gets the order by descending expression
    /// </summary>
    Expression<Func<T, object>>? OrderByDescending { get; }
    
    /// <summary>
    /// Gets the group by expression
    /// </summary>
    Expression<Func<T, object>>? GroupBy { get; }
    
    /// <summary>
    /// Gets the number of items to take (for paging)
    /// </summary>
    int Take { get; }
    
    /// <summary>
    /// Gets the number of items to skip (for paging)
    /// </summary>
    int Skip { get; }
    
    /// <summary>
    /// Gets whether paging is enabled
    /// </summary>
    bool IsPagingEnabled { get; }
}