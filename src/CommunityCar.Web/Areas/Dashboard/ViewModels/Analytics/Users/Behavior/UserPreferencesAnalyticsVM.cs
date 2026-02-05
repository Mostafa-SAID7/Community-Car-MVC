namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Analytics.Users.Behavior;

/// <summary>
/// ViewModel for user preferences analytics
/// </summary>
public class UserPreferencesAnalyticsVM
{
    public List<ContentPreferenceVM> ContentPreferences { get; set; } = new();
    public List<FeaturePreferenceVM> FeaturePreferences { get; set; } = new();
    public List<CommunicationPreferenceVM> CommunicationPreferences { get; set; } = new();
    public List<PrivacyPreferenceVM> PrivacyPreferences { get; set; } = new();
    public List<PersonalizationInsightVM> PersonalizationInsights { get; set; } = new();
}

/// <summary>
/// ViewModel for content preferences
/// </summary>
public class ContentPreferenceVM
{
    public string ContentType { get; set; } = string.Empty;
    public string ContentCategory { get; set; } = string.Empty;
    public int UserCount { get; set; }
    public decimal PreferencePercentage { get; set; }
    public decimal EngagementRate { get; set; }
    public List<ContentPreferenceTrendVM> Trends { get; set; } = new();
}

/// <summary>
/// ViewModel for content preference trends
/// </summary>
public class ContentPreferenceTrendVM
{
    public DateTime Date { get; set; }
    public int UserCount { get; set; }
    public decimal EngagementRate { get; set; }
    public string Period { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel for feature preferences
/// </summary>
public class FeaturePreferenceVM
{
    public string FeatureName { get; set; } = string.Empty;
    public string FeatureCategory { get; set; } = string.Empty;
    public int EnabledCount { get; set; }
    public int DisabledCount { get; set; }
    public decimal AdoptionRate { get; set; }
    public decimal SatisfactionScore { get; set; }
    public List<FeatureUsagePatternVM> UsagePatterns { get; set; } = new();
}

/// <summary>
/// ViewModel for feature usage patterns
/// </summary>
public class FeatureUsagePatternVM
{
    public string UsagePattern { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int UserCount { get; set; }
    public decimal Percentage { get; set; }
}

/// <summary>
/// ViewModel for communication preferences
/// </summary>
public class CommunicationPreferenceVM
{
    public string CommunicationType { get; set; } = string.Empty; // Email, SMS, Push, etc.
    public string CommunicationCategory { get; set; } = string.Empty;
    public int OptedInCount { get; set; }
    public int OptedOutCount { get; set; }
    public decimal OptInRate { get; set; }
    public decimal EngagementRate { get; set; }
    public List<CommunicationFrequencyVM> FrequencyPreferences { get; set; } = new();
}

/// <summary>
/// ViewModel for communication frequency preferences
/// </summary>
public class CommunicationFrequencyVM
{
    public string Frequency { get; set; } = string.Empty; // Daily, Weekly, Monthly, Never
    public int UserCount { get; set; }
    public decimal Percentage { get; set; }
}

/// <summary>
/// ViewModel for privacy preferences
/// </summary>
public class PrivacyPreferenceVM
{
    public string PrivacySetting { get; set; } = string.Empty;
    public string SettingCategory { get; set; } = string.Empty;
    public int EnabledCount { get; set; }
    public int DisabledCount { get; set; }
    public decimal EnabledPercentage { get; set; }
    public List<PrivacyTrendVM> Trends { get; set; } = new();
}

/// <summary>
/// ViewModel for privacy trends
/// </summary>
public class PrivacyTrendVM
{
    public DateTime Date { get; set; }
    public int EnabledCount { get; set; }
    public decimal EnabledPercentage { get; set; }
    public string Period { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel for personalization insights
/// </summary>
public class PersonalizationInsightVM
{
    public string InsightType { get; set; } = string.Empty;
    public string InsightCategory { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal ImpactScore { get; set; }
    public decimal ConfidenceLevel { get; set; }
    public List<PersonalizationRecommendationVM> Recommendations { get; set; } = new();
    public List<PersonalizationMetricVM> Metrics { get; set; } = new();
}

/// <summary>
/// ViewModel for personalization recommendations
/// </summary>
public class PersonalizationRecommendationVM
{
    public string RecommendationType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal ExpectedImpact { get; set; }
    public string Priority { get; set; } = string.Empty; // High, Medium, Low
    public List<string> TargetSegments { get; set; } = new();
}

/// <summary>
/// ViewModel for personalization metrics
/// </summary>
public class PersonalizationMetricVM
{
    public string MetricName { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal Benchmark { get; set; }
    public decimal PerformanceScore { get; set; }
    public string PerformanceLevel { get; set; } = string.Empty;
}




