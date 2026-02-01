namespace CommunityCar.Domain.Enums.Community;

/// <summary>
/// Feed sorting options
/// </summary>
public enum FeedSortBy
{
    /// <summary>
    /// Sort by creation date (newest first)
    /// </summary>
    Newest = 0,
    
    /// <summary>
    /// Sort by creation date (oldest first)
    /// </summary>
    Oldest = 1,
    
    /// <summary>
    /// Sort by popularity (most liked/engaged)
    /// </summary>
    Popular = 2,
    
    /// <summary>
    /// Sort by trending (recent engagement)
    /// </summary>
    Trending = 3,
    
    /// <summary>
    /// Sort by most commented
    /// </summary>
    MostCommented = 4,
    
    /// <summary>
    /// Sort by most viewed
    /// </summary>
    MostViewed = 5,
    
    /// <summary>
    /// Sort by most shared
    /// </summary>
    MostShared = 6,
    
    /// <summary>
    /// Sort by engagement (likes + comments + shares)
    /// </summary>
    Engagement = 7,
    
    /// <summary>
    /// Sort by relevance (algorithm-based)
    /// </summary>
    Relevance = 8,
    
    /// <summary>
    /// Sort by author name
    /// </summary>
    Author = 9,
    
    /// <summary>
    /// Sort by category
    /// </summary>
    Category = 10,
    
    /// <summary>
    /// Default sorting (usually newest)
    /// </summary>
    Default = Newest
}