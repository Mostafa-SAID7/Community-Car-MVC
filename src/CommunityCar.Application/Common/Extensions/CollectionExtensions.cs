namespace CommunityCar.Application.Common.Extensions;

/// <summary>
/// Extensions for collections and enumerables
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Checks if collection is null or empty
    /// </summary>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? collection)
    {
        return collection == null || !collection.Any();
    }

    /// <summary>
    /// Checks if collection has any items
    /// </summary>
    public static bool HasItems<T>(this IEnumerable<T>? collection)
    {
        return !collection.IsNullOrEmpty();
    }

    /// <summary>
    /// Safely gets an item at index or returns default
    /// </summary>
    public static T? SafeElementAt<T>(this IEnumerable<T> collection, int index)
    {
        return collection.Skip(index).FirstOrDefault();
    }

    /// <summary>
    /// Chunks collection into smaller collections of specified size
    /// </summary>
    public static IEnumerable<IEnumerable<T>> ChunkBy<T>(this IEnumerable<T> collection, int chunkSize)
    {
        if (chunkSize <= 0)
            throw new ArgumentException("Chunk size must be greater than 0", nameof(chunkSize));

        var list = collection.ToList();
        for (int i = 0; i < list.Count; i += chunkSize)
        {
            yield return list.Skip(i).Take(chunkSize);
        }
    }

    /// <summary>
    /// Performs an action on each item in the collection
    /// </summary>
    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> action)
    {
        var list = collection.ToList();
        foreach (var item in list)
        {
            action(item);
        }
        return list;
    }

    /// <summary>
    /// Performs an async action on each item in the collection
    /// </summary>
    public static async Task<IEnumerable<T>> ForEachAsync<T>(
        this IEnumerable<T> collection,
        Func<T, Task> action)
    {
        var list = collection.ToList();
        foreach (var item in list)
        {
            await action(item);
        }
        return list;
    }

    /// <summary>
    /// Performs an async action on each item in parallel
    /// </summary>
    public static async Task<IEnumerable<T>> ForEachParallelAsync<T>(
        this IEnumerable<T> collection,
        Func<T, Task> action,
        int maxDegreeOfParallelism = -1)
    {
        var list = collection.ToList();
        var processorCount = maxDegreeOfParallelism == -1 ? Environment.ProcessorCount : maxDegreeOfParallelism;
        var semaphore = new SemaphoreSlim(processorCount);
        
        var tasks = list.Select(async item =>
        {
            await semaphore.WaitAsync();
            try
            {
                await action(item);
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
        return list;
    }

    /// <summary>
    /// Removes duplicates based on a key selector
    /// </summary>
    public static IEnumerable<T> DistinctBy<T, TKey>(
        this IEnumerable<T> collection,
        Func<T, TKey> keySelector)
    {
        var seen = new HashSet<TKey>();
        return collection.Where(item => seen.Add(keySelector(item)));
    }

    /// <summary>
    /// Converts collection to dictionary safely, handling duplicate keys
    /// </summary>
    public static Dictionary<TKey, TValue> ToSafeDictionary<T, TKey, TValue>(
        this IEnumerable<T> collection,
        Func<T, TKey> keySelector,
        Func<T, TValue> valueSelector,
        Func<TValue, TValue, TValue>? duplicateHandler = null)
        where TKey : notnull
    {
        var dictionary = new Dictionary<TKey, TValue>();
        
        foreach (var item in collection)
        {
            var key = keySelector(item);
            var value = valueSelector(item);
            
            if (dictionary.ContainsKey(key))
            {
                if (duplicateHandler != null)
                {
                    dictionary[key] = duplicateHandler(dictionary[key], value);
                }
                // Otherwise, keep the first value (ignore duplicates)
            }
            else
            {
                dictionary[key] = value;
            }
        }
        
        return dictionary;
    }

    /// <summary>
    /// Finds the item with the maximum value based on a selector
    /// </summary>
    public static T? MaxBy<T, TKey>(this IEnumerable<T> collection, Func<T, TKey> selector)
        where TKey : IComparable<TKey>
    {
        using var enumerator = collection.GetEnumerator();
        
        if (!enumerator.MoveNext())
            return default;

        var maxItem = enumerator.Current;
        var maxValue = selector(maxItem);

        while (enumerator.MoveNext())
        {
            var currentValue = selector(enumerator.Current);
            if (currentValue.CompareTo(maxValue) > 0)
            {
                maxItem = enumerator.Current;
                maxValue = currentValue;
            }
        }

        return maxItem;
    }

    /// <summary>
    /// Finds the item with the minimum value based on a selector
    /// </summary>
    public static T? MinBy<T, TKey>(this IEnumerable<T> collection, Func<T, TKey> selector)
        where TKey : IComparable<TKey>
    {
        using var enumerator = collection.GetEnumerator();
        
        if (!enumerator.MoveNext())
            return default;

        var minItem = enumerator.Current;
        var minValue = selector(minItem);

        while (enumerator.MoveNext())
        {
            var currentValue = selector(enumerator.Current);
            if (currentValue.CompareTo(minValue) < 0)
            {
                minItem = enumerator.Current;
                minValue = currentValue;
            }
        }

        return minItem;
    }

    /// <summary>
    /// Randomly shuffles the collection
    /// </summary>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection)
    {
        var list = collection.ToList();
        var random = new Random();
        
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
        
        return list;
    }

    /// <summary>
    /// Takes a random sample from the collection
    /// </summary>
    public static IEnumerable<T> TakeRandom<T>(this IEnumerable<T> collection, int count)
    {
        return collection.Shuffle().Take(count);
    }

    /// <summary>
    /// Partitions collection into two based on a predicate
    /// </summary>
    public static (IEnumerable<T> Matching, IEnumerable<T> NotMatching) Partition<T>(
        this IEnumerable<T> collection,
        Func<T, bool> predicate)
    {
        var list = collection.ToList();
        var matching = new List<T>();
        var notMatching = new List<T>();

        foreach (var item in list)
        {
            if (predicate(item))
                matching.Add(item);
            else
                notMatching.Add(item);
        }

        return (matching, notMatching);
    }

    /// <summary>
    /// Converts async enumerable to list
    /// </summary>
    public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> asyncEnumerable)
    {
        var list = new List<T>();
        await foreach (var item in asyncEnumerable)
        {
            list.Add(item);
        }
        return list;
    }

    /// <summary>
    /// Adds range of items to collection if it supports it
    /// </summary>
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        if (collection is List<T> list)
        {
            list.AddRange(items);
        }
        else
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }
    }
}