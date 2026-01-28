using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.ValueObjects.Account;

/// <summary>
/// Value object representing profile view metrics
/// </summary>
public class ProfileViewMetrics : ValueObject
{
    public int TotalViews { get; private set; }
    public int UniqueViewers { get; private set; }
    public int TodayViews { get; private set; }
    public int WeekViews { get; private set; }
    public int MonthViews { get; private set; }
    public TimeSpan AverageViewDuration { get; private set; }
    public DateTime? LastViewedAt { get; private set; }

    public ProfileViewMetrics(
        int totalViews = 0,
        int uniqueViewers = 0,
        int todayViews = 0,
        int weekViews = 0,
        int monthViews = 0,
        TimeSpan averageViewDuration = default,
        DateTime? lastViewedAt = null)
    {
        TotalViews = Math.Max(0, totalViews);
        UniqueViewers = Math.Max(0, uniqueViewers);
        TodayViews = Math.Max(0, todayViews);
        WeekViews = Math.Max(0, weekViews);
        MonthViews = Math.Max(0, monthViews);
        AverageViewDuration = averageViewDuration;
        LastViewedAt = lastViewedAt;
    }

    public ProfileViewMetrics AddView(bool isUniqueViewer, TimeSpan viewDuration, DateTime viewedAt)
    {
        var newTotalViews = TotalViews + 1;
        var newUniqueViewers = isUniqueViewer ? UniqueViewers + 1 : UniqueViewers;
        var newTodayViews = viewedAt.Date == DateTime.Today ? TodayViews + 1 : TodayViews;
        var newWeekViews = viewedAt >= DateTime.Today.AddDays(-7) ? WeekViews + 1 : WeekViews;
        var newMonthViews = viewedAt >= DateTime.Today.AddDays(-30) ? MonthViews + 1 : MonthViews;

        // Calculate new average duration
        var totalDurationSeconds = (AverageViewDuration.TotalSeconds * (TotalViews - 1)) + viewDuration.TotalSeconds;
        var newAverageDuration = TimeSpan.FromSeconds(totalDurationSeconds / newTotalViews);

        return new ProfileViewMetrics(
            newTotalViews,
            newUniqueViewers,
            newTodayViews,
            newWeekViews,
            newMonthViews,
            newAverageDuration,
            viewedAt);
    }

    public double ViewerReturnRate => UniqueViewers > 0 ? (double)TotalViews / UniqueViewers : 0;

    public bool IsPopularProfile => TotalViews > 100 && UniqueViewers > 50;

    public bool HasRecentActivity => LastViewedAt.HasValue && 
                                   DateTime.UtcNow - LastViewedAt.Value < TimeSpan.FromDays(7);

    public ProfileViewTrend GetTrend()
    {
        if (WeekViews > MonthViews * 0.5)
            return ProfileViewTrend.Increasing;
        
        if (WeekViews < MonthViews * 0.1)
            return ProfileViewTrend.Decreasing;
        
        return ProfileViewTrend.Stable;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return TotalViews;
        yield return UniqueViewers;
        yield return TodayViews;
        yield return WeekViews;
        yield return MonthViews;
        yield return AverageViewDuration;
        yield return LastViewedAt;
    }
}

public enum ProfileViewTrend
{
    Increasing,
    Stable,
    Decreasing
}