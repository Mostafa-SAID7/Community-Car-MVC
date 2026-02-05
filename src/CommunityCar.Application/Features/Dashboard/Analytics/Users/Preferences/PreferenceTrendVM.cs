namespace CommunityCar.Application.Features.Dashboard.Analytics.Users.Preferences;

/// <summary>
/// Preference trend view model
/// </summary>
public class PreferenceTrendVM
{
    public string PreferenceName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int Count { get; set; }
    public double Percentage { get; set; }
    public string TrendDirection { get; set; } = string.Empty; // Up, Down, Stable
}

/// <summary>
/// Preference insight view model
/// </summary>
public class PreferenceInsightVM
{
    public string Category { get; set; } = string.Empty;
    public string Insight { get; set; } = string.Empty;
    public double Impact { get; set; }
    public List<string> RecommendedActions { get; set; } = new();
}

/// <summary>
/// Category statistics view model
/// </summary>
public class CategoryStatsVM
{
    public string CategoryName { get; set; } = string.Empty;
    public int UserCount { get; set; }
    public double Percentage { get; set; }
    public double EngagementRate { get; set; }
}

/// <summary>
/// Tag statistics view model
/// </summary>
public class TagStatsVM
{
    public string TagName { get; set; } = string.Empty;
    public int UsageCount { get; set; }
    public double Popularity { get; set; }
    public List<string> RelatedTags { get; set; } = new();
}

/// <summary>
/// Preference distribution view model
/// </summary>
public class PreferenceDistributionVM
{
    public Dictionary<string, int> CategoryDistribution { get; set; } = new();
    public Dictionary<string, int> TagDistribution { get; set; } = new();
    public Dictionary<string, double> EngagementByCategory { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}