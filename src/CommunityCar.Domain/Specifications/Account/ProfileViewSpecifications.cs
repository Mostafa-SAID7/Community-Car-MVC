using CommunityCar.Domain.Entities.Account.Profile;

namespace CommunityCar.Domain.Specifications.Account;

/// <summary>
/// Specifications for UserProfileView entity queries
/// </summary>
public static class ProfileViewSpecifications
{
    /// <summary>
    /// Specification for profile views by profile user
    /// </summary>
    public class ViewsByProfileUserSpec : BaseSpecification<UserProfileView>
    {
        public ViewsByProfileUserSpec(Guid profileUserId) 
            : base(v => v.ProfileUserId == profileUserId)
        {
            AddOrderByDescending(v => v.ViewedAt);
        }
    }

    /// <summary>
    /// Specification for profile views by viewer
    /// </summary>
    public class ViewsByViewerSpec : BaseSpecification<UserProfileView>
    {
        public ViewsByViewerSpec(Guid viewerId) 
            : base(v => v.ViewerId == viewerId && !v.IsAnonymous)
        {
            AddOrderByDescending(v => v.ViewedAt);
        }
    }

    /// <summary>
    /// Specification for recent profile views
    /// </summary>
    public class RecentViewsSpec : BaseSpecification<UserProfileView>
    {
        public RecentViewsSpec(Guid profileUserId, int minutesBack = 30) 
            : base(v => v.ProfileUserId == profileUserId && 
                       v.ViewedAt >= DateTime.UtcNow.AddMinutes(-minutesBack))
        {
            AddOrderByDescending(v => v.ViewedAt);
        }
    }

    /// <summary>
    /// Specification for views by source
    /// </summary>
    public class ViewsBySourceSpec : BaseSpecification<UserProfileView>
    {
        public ViewsBySourceSpec(Guid profileUserId, string viewSource) 
            : base(v => v.ProfileUserId == profileUserId && v.ViewSource == viewSource)
        {
            AddOrderByDescending(v => v.ViewedAt);
        }
    }

    /// <summary>
    /// Specification for non-anonymous views
    /// </summary>
    public class NonAnonymousViewsSpec : BaseSpecification<UserProfileView>
    {
        public NonAnonymousViewsSpec(Guid profileUserId) 
            : base(v => v.ProfileUserId == profileUserId && !v.IsAnonymous)
        {
            AddOrderByDescending(v => v.ViewedAt);
        }
    }

    /// <summary>
    /// Specification for views within date range
    /// </summary>
    public class ViewsInDateRangeSpec : BaseSpecification<UserProfileView>
    {
        public ViewsInDateRangeSpec(Guid profileUserId, DateTime startDate, DateTime endDate) 
            : base(v => v.ProfileUserId == profileUserId && 
                       v.ViewedAt >= startDate && 
                       v.ViewedAt <= endDate)
        {
            AddOrderByDescending(v => v.ViewedAt);
        }
    }

    /// <summary>
    /// Specification for long duration views
    /// </summary>
    public class LongDurationViewsSpec : BaseSpecification<UserProfileView>
    {
        public LongDurationViewsSpec(Guid profileUserId, int minSeconds = 30) 
            : base(v => v.ProfileUserId == profileUserId && 
                       v.ViewDuration.TotalSeconds >= minSeconds)
        {
            AddOrderByDescending(v => v.ViewDuration);
        }
    }

    /// <summary>
    /// Specification for views from specific location
    /// </summary>
    public class ViewsByLocationSpec : BaseSpecification<UserProfileView>
    {
        public ViewsByLocationSpec(Guid profileUserId, string location) 
            : base(v => v.ProfileUserId == profileUserId && 
                       !string.IsNullOrEmpty(v.ViewerLocation) && 
                       v.ViewerLocation.Contains(location))
        {
            AddOrderByDescending(v => v.ViewedAt);
        }
    }
}