namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Analytics.Users.Segments;

/// <summary>
/// ViewModel for user segmentation analytics
/// </summary>
public class UserSegmentationAnalyticsVM
{
    public List<UserSegmentAnalyticsVM> UserSegments { get; set; } = new();
    public List<SegmentComparisonVM> SegmentComparisons { get; set; } = new();
    public List<SegmentMigrationVM> SegmentMigrations { get; set; } = new();
    public List<SegmentPerformanceVM> SegmentPerformance { get; set; } = new();
}

/// <summary>
/// ViewModel for user segments in analytics
/// </summary>
public class UserSegmentAnalyticsVM
{
    public string SegmentId { get; set; } = string.Empty;
    public string SegmentName { get; set; } = string.Empty;
    public string SegmentDescription { get; set; } = string.Empty;
    public string SegmentType { get; set; } = string.Empty; // Behavioral, Demographic, Geographic
    public int UserCount { get; set; }
    public decimal Percentage { get; set; }
    public List<SegmentCriteriaVM> Criteria { get; set; } = new();
    public List<SegmentCharacteristicVM> Characteristics { get; set; } = new();
    public SegmentMetricsVM Metrics { get; set; } = new();
}

/// <summary>
/// ViewModel for segment criteria
/// </summary>
public class SegmentCriteriaVM
{
    public string CriteriaName { get; set; } = string.Empty;
    public string CriteriaType { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty; // Equals, GreaterThan, etc.
    public string Value { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel for segment characteristics
/// </summary>
public class SegmentCharacteristicVM
{
    public string CharacteristicName { get; set; } = string.Empty;
    public string CharacteristicValue { get; set; } = string.Empty;
    public decimal AverageValue { get; set; }
    public string Unit { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel for segment metrics
/// </summary>
public class SegmentMetricsVM
{
    public decimal EngagementRate { get; set; }
    public decimal RetentionRate { get; set; }
    public decimal ConversionRate { get; set; }
    public decimal LifetimeValue { get; set; }
    public TimeSpan AverageSessionDuration { get; set; }
    public int AveragePageViews { get; set; }
    public decimal ChurnRate { get; set; }
}

/// <summary>
/// ViewModel for segment comparisons
/// </summary>
public class SegmentComparisonVM
{
    public string MetricName { get; set; } = string.Empty;
    public List<SegmentMetricComparisonVM> SegmentValues { get; set; } = new();
    public string BestPerformingSegment { get; set; } = string.Empty;
    public string WorstPerformingSegment { get; set; } = string.Empty;
    public decimal PerformanceGap { get; set; }
}

/// <summary>
/// ViewModel for segment metric comparison
/// </summary>
public class SegmentMetricComparisonVM
{
    public string SegmentName { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public int Rank { get; set; }
    public decimal DifferenceFromBest { get; set; }
}

/// <summary>
/// ViewModel for segment migrations
/// </summary>
public class SegmentMigrationVM
{
    public string FromSegment { get; set; } = string.Empty;
    public string ToSegment { get; set; } = string.Empty;
    public int UserCount { get; set; }
    public decimal MigrationRate { get; set; }
    public List<MigrationReasonVM> Reasons { get; set; } = new();
    public DateTime AnalysisPeriodStart { get; set; }
    public DateTime AnalysisPeriodEnd { get; set; }
}

/// <summary>
/// ViewModel for migration reasons
/// </summary>
public class MigrationReasonVM
{
    public string Reason { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}

/// <summary>
/// ViewModel for segment performance
/// </summary>
public class SegmentPerformanceVM
{
    public string SegmentName { get; set; } = string.Empty;
    public List<PerformanceMetricVM> Metrics { get; set; } = new();
    public List<PerformanceTrendVM> Trends { get; set; } = new();
    public string PerformanceGrade { get; set; } = string.Empty; // A, B, C, D, F
    public List<string> Strengths { get; set; } = new();
    public List<string> Weaknesses { get; set; } = new();
    public List<string> Opportunities { get; set; } = new();
}

/// <summary>
/// ViewModel for performance metrics
/// </summary>
public class PerformanceMetricVM
{
    public string MetricName { get; set; } = string.Empty;
    public decimal CurrentValue { get; set; }
    public decimal BenchmarkValue { get; set; }
    public decimal PerformanceScore { get; set; }
    public string PerformanceLevel { get; set; } = string.Empty; // Excellent, Good, Average, Poor
}

/// <summary>
/// ViewModel for performance trends
/// </summary>
public class PerformanceTrendVM
{
    public string MetricName { get; set; } = string.Empty;
    public List<TrendDataPointVM> DataPoints { get; set; } = new();
    public string TrendDirection { get; set; } = string.Empty;
    public decimal TrendStrength { get; set; }
}

/// <summary>
/// ViewModel for trend data points
/// </summary>
public class TrendDataPointVM
{
    public DateTime Date { get; set; }
    public decimal Value { get; set; }
    public string Label { get; set; } = string.Empty;
}




