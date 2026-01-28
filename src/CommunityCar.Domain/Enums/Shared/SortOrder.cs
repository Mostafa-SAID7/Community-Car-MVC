namespace CommunityCar.Domain.Enums.Shared;

/// <summary>
/// Represents sort order options
/// </summary>
public enum SortOrder
{
    /// <summary>
    /// Ascending order (A-Z, 1-9, oldest first)
    /// </summary>
    Ascending = 0,
    
    /// <summary>
    /// Descending order (Z-A, 9-1, newest first)
    /// </summary>
    Descending = 1
}

/// <summary>
/// Represents different sorting criteria
/// </summary>
public enum SortBy
{
    /// <summary>
    /// Sort by creation date
    /// </summary>
    CreatedAt = 0,
    
    /// <summary>
    /// Sort by last update date
    /// </summary>
    UpdatedAt = 1,
    
    /// <summary>
    /// Sort by name/title
    /// </summary>
    Name = 2,
    
    /// <summary>
    /// Sort by popularity (views, likes, etc.)
    /// </summary>
    Popularity = 3,
    
    /// <summary>
    /// Sort by rating/score
    /// </summary>
    Rating = 4,
    
    /// <summary>
    /// Sort by relevance (search results)
    /// </summary>
    Relevance = 5,
    
    /// <summary>
    /// Sort by priority
    /// </summary>
    Priority = 6,
    
    /// <summary>
    /// Sort by status
    /// </summary>
    Status = 7
}