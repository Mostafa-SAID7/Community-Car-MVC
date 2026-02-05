using CommunityCar.Web.Areas.Identity.Interfaces.Repositories.Authorization;

namespace CommunityCar.Web.Areas.Identity.Interfaces.Repositories;

public interface IIdentityUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IUserSessionRepository UserSessions { get; }
    IUserTokenRepository UserTokens { get; }
    IUserActivityRepository UserActivities { get; }
    IUserProfileViewRepository UserProfileViews { get; }
    IUserAchievementRepository UserAchievements { get; }
    IUserBadgeRepository UserBadges { get; }
    IUserFollowingRepository UserFollowings { get; }
    IUserInterestRepository UserInterests { get; }
    IUserGalleryRepository UserGalleries { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
