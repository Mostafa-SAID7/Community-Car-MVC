using CommunityCar.Web.Areas.Identity.Interfaces.Repositories;
using CommunityCar.Infrastructure.Persistence.Data;

namespace CommunityCar.Web.Areas.Identity.Repositories;

public class IdentityUnitOfWork : IIdentityUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IdentityUnitOfWork(
        ApplicationDbContext context,
        IUserRepository userRepository,
        IUserSessionRepository userSessionRepository,
        IUserTokenRepository userTokenRepository,
        IUserActivityRepository userActivityRepository,
        IUserProfileViewRepository userProfileViewRepository,
        IUserAchievementRepository userAchievementRepository,
        IUserBadgeRepository userBadgeRepository,
        IUserFollowingRepository userFollowingRepository,
        IUserInterestRepository userInterestRepository,
        IUserGalleryRepository userGalleryRepository)
    {
        _context = context;
        Users = userRepository;
        UserSessions = userSessionRepository;
        UserTokens = userTokenRepository;
        UserActivities = userActivityRepository;
        UserProfileViews = userProfileViewRepository;
        UserAchievements = userAchievementRepository;
        UserBadges = userBadgeRepository;
        UserFollowings = userFollowingRepository;
        UserInterests = userInterestRepository;
        UserGalleries = userGalleryRepository;
    }

    public IUserRepository Users { get; }
    public IUserSessionRepository UserSessions { get; }
    public IUserTokenRepository UserTokens { get; }
    public IUserActivityRepository UserActivities { get; }
    public IUserProfileViewRepository UserProfileViews { get; }
    public IUserAchievementRepository UserAchievements { get; }
    public IUserBadgeRepository UserBadges { get; }
    public IUserFollowingRepository UserFollowings { get; }
    public IUserInterestRepository UserInterests { get; }
    public IUserGalleryRepository UserGalleries { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
