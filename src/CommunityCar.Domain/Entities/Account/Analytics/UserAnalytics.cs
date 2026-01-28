using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Account.Analytics;

/// <summary>
/// Aggregated daily analytics for all users
/// </summary>
public class UserAnalytics : BaseEntity
{
    public DateTime Date { get; private set; }
    public int NewUsers { get; private set; }
    public int ActiveUsers { get; private set; }
    public int ReturnUsers { get; private set; }
    public decimal RetentionRate { get; private set; }
    public decimal ChurnRate { get; private set; }
    public TimeSpan AverageSessionDuration { get; private set; }
    public int PageViews { get; private set; }
    public int UniquePageViews { get; private set; }
    public decimal BounceRate { get; private set; }

    public UserAnalytics(DateTime date)
    {
        Date = date.Date;
    }

    private UserAnalytics() { }

    public void UpdateMetrics(int newUsers, int activeUsers, int returnUsers,
        decimal retentionRate, decimal churnRate, TimeSpan avgSessionDuration,
        int pageViews, int uniquePageViews, decimal bounceRate)
    {
        NewUsers = newUsers;
        ActiveUsers = activeUsers;
        ReturnUsers = returnUsers;
        RetentionRate = retentionRate;
        ChurnRate = churnRate;
        AverageSessionDuration = avgSessionDuration;
        PageViews = pageViews;
        UniquePageViews = uniquePageViews;
        BounceRate = bounceRate;
        Audit(UpdatedBy);
    }
}