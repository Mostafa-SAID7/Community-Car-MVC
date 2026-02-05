using CommunityCar.Web.Areas.Identity.Interfaces.Repositories;
using CommunityCar.Web.Areas.Identity.Interfaces.Services.Authentication;
using CommunityCar.Web.Areas.Identity.Interfaces.Services.Authentication.OAuth;
using CommunityCar.Web.Areas.Identity.Interfaces.Services.Core;
using CommunityCar.Web.Areas.Identity.Interfaces.Services.Gamification;
using CommunityCar.Web.Areas.Identity.Interfaces.Services.Media;
using CommunityCar.Web.Areas.Identity.Interfaces.Services.Profile;
using CommunityCar.Web.Areas.Identity.Interfaces.Services.Security;
using CommunityCar.Web.Areas.Identity.Repositories;
using CommunityCar.Web.Areas.Identity.Repositories.Activity;
using CommunityCar.Web.Areas.Identity.Repositories.Authentication;
using CommunityCar.Web.Areas.Identity.Repositories.Core;
using CommunityCar.Web.Areas.Identity.Repositories.Gamification;
using CommunityCar.Web.Areas.Identity.Repositories.Media;
using CommunityCar.Web.Areas.Identity.Repositories.Social;
using CommunityCar.Web.Areas.Identity.Services.Authentication;
using CommunityCar.Web.Areas.Identity.Services.Authentication.OAuth;
using CommunityCar.Web.Areas.Identity.Services.Core;
using CommunityCar.Web.Areas.Identity.Services.Gamification;
using CommunityCar.Web.Areas.Identity.Services.Media;
using CommunityCar.Web.Areas.Identity.Services.Profile;
using CommunityCar.Web.Areas.Identity.Services.Security;
using Microsoft.Extensions.DependencyInjection;

namespace CommunityCar.Web.Areas.Identity.Configuration;

public static class IdentityDependencyInjection
{
    public static IServiceCollection AddIdentityAreaServices(this IServiceCollection services)
    {
        // Unit of Work
        services.AddScoped<IIdentityUnitOfWork, IdentityUnitOfWork>();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserSessionRepository, UserSessionRepository>();
        services.AddScoped<IUserTokenRepository, UserTokenRepository>();
        services.AddScoped<IUserActivityRepository, UserActivityRepository>();
        services.AddScoped<IUserProfileViewRepository, UserProfileViewRepository>();
        services.AddScoped<IUserAchievementRepository, UserAchievementRepository>();
        services.AddScoped<IUserBadgeRepository, UserBadgeRepository>();
        services.AddScoped<IUserFollowingRepository, UserFollowingRepository>();
        services.AddScoped<IUserInterestRepository, UserInterestRepository>();
        services.AddScoped<IUserGalleryRepository, UserGalleryRepository>();

        // Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IOAuthService, OAuthService>();
        services.AddScoped<IFacebookOAuthService, FacebookOAuthService>();
        services.AddScoped<IGoogleOAuthService, GoogleOAuthService>();
        services.AddScoped<ITwoFactorService, TwoFactorService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IGamificationService, GamificationService>();
        services.AddScoped<IProgressionService, ProgressionService>();
        services.AddScoped<IUserGalleryService, UserGalleryService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IProfileViewService, ProfileViewService>();
        services.AddScoped<IAccountSecurityService, AccountSecurityService>();

        return services;
    }
}
