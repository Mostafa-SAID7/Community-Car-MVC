using AutoMapper;
using CommunityCar.Application.Common.Models;

namespace CommunityCar.Application.Common.Extensions;

/// <summary>
/// Extensions for mapping operations
/// </summary>
public static class MappingExtensions
{
    /// <summary>
    /// Maps a collection to another type
    /// </summary>
    public static IEnumerable<TDestination> MapTo<TDestination>(
        this IEnumerable<object> source,
        IMapper mapper)
    {
        return mapper.Map<IEnumerable<TDestination>>(source);
    }

    /// <summary>
    /// Maps a single object to another type
    /// </summary>
    public static TDestination MapTo<TDestination>(
        this object source,
        IMapper mapper)
    {
        return mapper.Map<TDestination>(source);
    }

    /// <summary>
    /// Maps source to existing destination object
    /// </summary>
    public static TDestination MapTo<TSource, TDestination>(
        this TSource source,
        TDestination destination,
        IMapper mapper)
    {
        return mapper.Map(source, destination);
    }

    /// <summary>
    /// Maps a paginated result to another type
    /// </summary>
    public static PaginatedResult<TDestination> MapTo<TSource, TDestination>(
        this PaginatedResult<TSource> source,
        IMapper mapper)
    {
        var mappedItems = mapper.Map<List<TDestination>>(source.Items);
        return new PaginatedResult<TDestination>(
            mappedItems,
            source.TotalCount,
            source.PageNumber,
            source.PageSize);
    }

    /// <summary>
    /// Maps a result with value to another type
    /// </summary>
    public static Result<TDestination> MapTo<TSource, TDestination>(
        this Result<TSource> source,
        IMapper mapper)
    {
        return source.IsSuccess
            ? Result<TDestination>.Success(mapper.Map<TDestination>(source.Value))
            : Result<TDestination>.Failure(source.Errors);
    }

    /// <summary>
    /// Conditionally maps based on a predicate
    /// </summary>
    public static TDestination? MapIf<TSource, TDestination>(
        this TSource source,
        Func<TSource, bool> predicate,
        IMapper mapper)
        where TDestination : class
    {
        return predicate(source) ? mapper.Map<TDestination>(source) : null;
    }

    /// <summary>
    /// Maps with custom configuration
    /// </summary>
    public static TDestination MapWith<TSource, TDestination>(
        this TSource source,
        IMapper mapper,
        Action<IMappingOperationOptions> opts)
    {
        return mapper.Map<TDestination>(source, opts);
    }

    /// <summary>
    /// Maps collection with filtering
    /// </summary>
    public static IEnumerable<TDestination> MapWhere<TSource, TDestination>(
        this IEnumerable<TSource> source,
        Func<TSource, bool> predicate,
        IMapper mapper)
    {
        var filtered = source.Where(predicate);
        return mapper.Map<IEnumerable<TDestination>>(filtered);
    }

    /// <summary>
    /// Maps and flattens nested collections
    /// </summary>
    public static IEnumerable<TDestination> MapMany<TSource, TDestination>(
        this IEnumerable<TSource> source,
        Func<TSource, IEnumerable<object>> selector,
        IMapper mapper)
    {
        var flattened = source.SelectMany(selector);
        return mapper.Map<IEnumerable<TDestination>>(flattened);
    }

    /// <summary>
    /// Maps to dictionary with key selector
    /// </summary>
    public static Dictionary<TKey, TDestination> MapToDictionary<TSource, TKey, TDestination>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        IMapper mapper)
        where TKey : notnull
    {
        return source.ToDictionary(keySelector, item => mapper.Map<TDestination>(item));
    }

    /// <summary>
    /// Maps grouped data
    /// </summary>
    public static IEnumerable<IGrouping<TKey, TDestination>> MapGrouped<TSource, TKey, TDestination>(
        this IEnumerable<IGrouping<TKey, TSource>> source,
        IMapper mapper)
    {
        return source.Select(group => 
            new Grouping<TKey, TDestination>(
                group.Key, 
                mapper.Map<IEnumerable<TDestination>>(group)));
    }

    /// <summary>
    /// Maps with null safety
    /// </summary>
    public static TDestination? MapSafe<TSource, TDestination>(
        this TSource? source,
        IMapper mapper)
        where TSource : class
        where TDestination : class
    {
        return source != null ? mapper.Map<TDestination>(source) : null;
    }

    /// <summary>
    /// Maps collection with null safety
    /// </summary>
    public static IEnumerable<TDestination> MapSafe<TSource, TDestination>(
        this IEnumerable<TSource>? source,
        IMapper mapper)
        where TSource : class
    {
        if (source == null) return Enumerable.Empty<TDestination>();
        var filtered = source.Where(x => x != null);
        return mapper.Map<IEnumerable<TDestination>>(filtered);
    }

    /// <summary>
    /// Maps with exception handling
    /// </summary>
    public static Result<TDestination> TryMap<TSource, TDestination>(
        this TSource source,
        IMapper mapper)
    {
        try
        {
            var result = mapper.Map<TDestination>(source);
            return Result<TDestination>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<TDestination>.Failure($"Mapping failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Maps collection with exception handling
    /// </summary>
    public static Result<IEnumerable<TDestination>> TryMapCollection<TSource, TDestination>(
        this IEnumerable<TSource> source,
        IMapper mapper)
    {
        try
        {
            var results = mapper.Map<IEnumerable<TDestination>>(source);
            return Result<IEnumerable<TDestination>>.Success(results);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<TDestination>>.Failure($"Collection mapping failed: {ex.Message}");
        }
    }
}

/// <summary>
/// Helper class for grouped mapping
/// </summary>
internal class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
{
    public TKey Key { get; }
    private readonly IEnumerable<TElement> _elements;

    public Grouping(TKey key, IEnumerable<TElement> elements)
    {
        Key = key;
        _elements = elements;
    }

    public IEnumerator<TElement> GetEnumerator() => _elements.GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}