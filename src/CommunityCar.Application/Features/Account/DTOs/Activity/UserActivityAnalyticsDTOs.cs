namespace CommunityCar.Application.Features.Account.DTOs.Activity;

#region User Activity Analytics DTOs

public class ActivityAnalyticsDTO
{
    public Guid UserId { get; set; }
    public int TotalActivities { get; set; }
    public Dictionary<string, int> ActivitiesByType { get; set; } = new();
    public DateTime? LastActivityDate { get; set; }
    public List<ActivityTrendDTO> ActivityTrends { get; set; } = new();
}

public class ActivityTrendDTO
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
    public string ActivityType { get; set; } = string.Empty;
}

public class ActivitySummaryDTO
{
    public Guid UserId { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public int TotalActivities { get; set; }
    public int UniqueDays { get; set; }
    public double AverageActivitiesPerDay { get; set; }
    public string MostActiveDay { get; set; } = string.Empty;
    public string MostCommonActivity { get; set; } = string.Empty;
    public Dictionary<string, int> HourlyDistribution { get; set; } = new();
    public Dictionary<string, int> DayOfWeekDistribution { get; set; } = new();
}

#endregion