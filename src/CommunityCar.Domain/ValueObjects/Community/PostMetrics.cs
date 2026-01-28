using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.ValueObjects.Community;

/// <summary>
/// Value object representing post engagement metrics
/// </summary>
public class PostMetrics : ValueObject
{
    public int ViewCount { get; private set; }
    public int LikeCount { get; private set; }
    public int CommentCount { get; private set; }
    public int ShareCount { get; private set; }
    public double EngagementRate { get; private set; }

    public PostMetrics(int viewCount = 0, int likeCount = 0, int commentCount = 0, int shareCount = 0)
    {
        ViewCount = Math.Max(0, viewCount);
        LikeCount = Math.Max(0, likeCount);
        CommentCount = Math.Max(0, commentCount);
        ShareCount = Math.Max(0, shareCount);
        EngagementRate = CalculateEngagementRate();
    }

    public PostMetrics IncrementViews(int count = 1)
    {
        return new PostMetrics(ViewCount + count, LikeCount, CommentCount, ShareCount);
    }

    public PostMetrics IncrementLikes(int count = 1)
    {
        return new PostMetrics(ViewCount, LikeCount + count, CommentCount, ShareCount);
    }

    public PostMetrics IncrementComments(int count = 1)
    {
        return new PostMetrics(ViewCount, LikeCount, CommentCount + count, ShareCount);
    }

    public PostMetrics IncrementShares(int count = 1)
    {
        return new PostMetrics(ViewCount, LikeCount, CommentCount, ShareCount + count);
    }

    public PostMetrics DecrementLikes(int count = 1)
    {
        return new PostMetrics(ViewCount, Math.Max(0, LikeCount - count), CommentCount, ShareCount);
    }

    public PostMetrics DecrementComments(int count = 1)
    {
        return new PostMetrics(ViewCount, LikeCount, Math.Max(0, CommentCount - count), ShareCount);
    }

    public int TotalEngagements => LikeCount + CommentCount + ShareCount;

    public bool IsPopular => EngagementRate > 0.05 && TotalEngagements > 10;

    public bool IsViral => EngagementRate > 0.1 && ShareCount > LikeCount * 0.1;

    private double CalculateEngagementRate()
    {
        if (ViewCount == 0)
            return 0;

        return (double)TotalEngagements / ViewCount;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ViewCount;
        yield return LikeCount;
        yield return CommentCount;
        yield return ShareCount;
    }
}