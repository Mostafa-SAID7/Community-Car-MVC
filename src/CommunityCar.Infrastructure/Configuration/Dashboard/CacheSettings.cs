namespace CommunityCar.Infrastructure.Configuration;

public class CacheSettings
{
    public const string SectionName = "Cache";

    public bool EnableDistributedCache { get; set; } = false;
    public string? RedisConnectionString { get; set; }
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(30);
    
    public CacheExpirationSettings Expiration { get; set; } = new();
    public CacheKeySettings Keys { get; set; } = new();
}

public class CacheExpirationSettings
{
    public TimeSpan UserProfile { get; set; } = TimeSpan.FromMinutes(15);
    public TimeSpan UserStatistics { get; set; } = TimeSpan.FromMinutes(30);
    public TimeSpan Feed { get; set; } = TimeSpan.FromMinutes(5);
    public TimeSpan Posts { get; set; } = TimeSpan.FromMinutes(10);
    public TimeSpan News { get; set; } = TimeSpan.FromMinutes(15);
    public TimeSpan Events { get; set; } = TimeSpan.FromMinutes(20);
    public TimeSpan Gamification { get; set; } = TimeSpan.FromHours(1);
    public TimeSpan Analytics { get; set; } = TimeSpan.FromHours(6);
    public TimeSpan Maintenance { get; set; } = TimeSpan.FromMinutes(5);
}

public class CacheKeySettings
{
    public string UserProfilePrefix { get; set; } = "user_profile";
    public string UserStatisticsPrefix { get; set; } = "user_stats";
    public string FeedPrefix { get; set; } = "feed";
    public string PostsPrefix { get; set; } = "posts";
    public string NewsPrefix { get; set; } = "news";
    public string EventsPrefix { get; set; } = "events";
    public string GamificationPrefix { get; set; } = "gamification";
    public string AnalyticsPrefix { get; set; } = "analytics";
}
