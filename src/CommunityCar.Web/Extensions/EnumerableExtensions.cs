using Microsoft.AspNetCore.Mvc.Rendering;

namespace CommunityCar.Web.Extensions;

/// <summary>
/// Extension methods for IEnumerable to simplify common operations
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Converts an enumerable to SelectListItem collection
    /// </summary>
    public static IEnumerable<SelectListItem> ToSelectList<T>(
        this IEnumerable<T> items,
        Func<T, object> valueSelector,
        Func<T, string> textSelector,
        object? selectedValue = null)
    {
        return items.Select(item => new SelectListItem
        {
            Value = valueSelector(item)?.ToString(),
            Text = textSelector(item),
            Selected = selectedValue != null && valueSelector(item)?.Equals(selectedValue) == true
        });
    }

    /// <summary>
    /// Converts an enumerable of strings to SelectListItem collection
    /// </summary>
    public static IEnumerable<SelectListItem> ToSelectList(
        this IEnumerable<string> items,
        string? selectedValue = null)
    {
        return items.Select(item => new SelectListItem
        {
            Value = item,
            Text = item,
            Selected = selectedValue != null && item.Equals(selectedValue, StringComparison.OrdinalIgnoreCase)
        });
    }

    /// <summary>
    /// Converts key-value pairs to SelectListItem collection
    /// </summary>
    public static IEnumerable<SelectListItem> ToSelectList<TKey, TValue>(
        this IEnumerable<KeyValuePair<TKey, TValue>> items,
        object? selectedValue = null)
    {
        return items.Select(item => new SelectListItem
        {
            Value = item.Key?.ToString(),
            Text = item.Value?.ToString() ?? string.Empty,
            Selected = selectedValue != null && item.Key?.Equals(selectedValue) == true
        });
    }

    /// <summary>
    /// Converts an enumerable to a paginated result
    /// </summary>
    public static PagedResult<T> ToPagedList<T>(
        this IEnumerable<T> source,
        int pageNumber,
        int pageSize)
    {
        var totalCount = source.Count();
        var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        
        return new PagedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    /// <summary>
    /// Checks if the enumerable is null or empty
    /// </summary>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
    {
        return source == null || !source.Any();
    }

    /// <summary>
    /// Performs an action on each element
    /// </summary>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
        {
            action(item);
        }
    }

    /// <summary>
    /// Splits a collection into chunks of specified size
    /// </summary>
    public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize)
    {
        if (chunkSize <= 0)
            throw new ArgumentException("Chunk size must be greater than 0", nameof(chunkSize));

        var list = source.ToList();
        for (int i = 0; i < list.Count; i += chunkSize)
        {
            yield return list.Skip(i).Take(chunkSize);
        }
    }

    /// <summary>
    /// Returns distinct elements by a key selector
    /// </summary>
    public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
    {
        var seenKeys = new HashSet<TKey>();
        foreach (var element in source)
        {
            if (seenKeys.Add(keySelector(element)))
            {
                yield return element;
            }
        }
    }
}

/// <summary>
/// Represents a paginated result
/// </summary>
public class PagedResult<T>
{
    public IList<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}