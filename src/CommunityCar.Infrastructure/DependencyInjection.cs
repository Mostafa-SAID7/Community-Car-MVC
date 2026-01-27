using CommunityCar.Infrastructure.Configuration;
using CommunityCar.Application.Common.Interfaces.Services.Authentication;
using CommunityCar.Application.Common.Interfaces.Services.Communication;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Shared;
using CommunityCar.Application.Common.Interfaces.Data;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Application.Common.Interfaces.Repositories.User;
using CommunityCar.Application.Common.Interfaces.Repositories.Profile;
using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Application.Common.Interfaces.Services.Storage;
using CommunityCar.Application.Services.Shared;
using CommunityCar.Application.Services.AI;
using CommunityCar.Domain.Entities.Auth;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using CommunityCar.Infrastructure.Persistence.Repositories.Community;
using CommunityCar.Infrastructure.Persistence.Repositories.User;
using CommunityCar.Infrastructure.Persistence.Repositories.Profile;
using CommunityCar.Infrastructure.Persistence.Repositories.Shared;
using CommunityCar.Infrastructure.Persistence.Repositories.AI;
using CommunityCar.Infrastructure.Persistence.UnitOfWork;
using CommunityCar.Infrastructure.Services.Authentication;
using CommunityCar.Infrastructure.Services.Authentication.OAuth;
using CommunityCar.Infrastructure.Services.Authentication.TwoFactor;
using CommunityCar.Infrastructure.Services.Authentication.Registration;
using CommunityCar.Infrastructure.Services.Authentication.Login;
using CommunityCar.Infrastructure.Services.Authentication.PasswordReset;
using CommunityCar.Application.Services.AI.ModelManagement;
using CommunityCar.Application.Services.AI.Training;
using CommunityCar.Application.Services.AI.History;
using CommunityCar.Infrastructure.Services.Communication;
using CommunityCar.Infrastructure.Services.Community;
using CommunityCar.Infrastructure.Services.Identity;
using CommunityCar.Infrastructure.Services.Storage;
using CommunityCar.Infrastructure.Services.Dashboard;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard;
using CommunityCar.Application.Services.Communication;
using CommunityCar.Application.Common.Interfaces.Services.Caching;
using CommunityCar.Application.Common.Interfaces.Services.BackgroundJobs;
using CommunityCar.Infrastructure.Caching;
using CommunityCar.Infrastructure.BackgroundJobs;
using CommunityCar.Infrastructure.Configuration;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CommunityCar.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString,
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
                    .EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null)));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddIdentity<User, IdentityRole<Guid>>(options => {
            // Password policy
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequiredUniqueChars = 4;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.RequireUniqueEmail = true;
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

            // Email confirmation
            options.SignIn.RequireConfirmedEmail = true;
            options.SignIn.RequireConfirmedAccount = false;

            // Token settings
            options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
            options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IQARepository, QARepository>();
        services.AddScoped<IVoteRepository, VoteRepository>();
        services.AddScoped<IViewRepository, ViewRepository>();
        services.AddScoped<IBookmarkRepository, BookmarkRepository>();
        services.AddScoped<IReactionRepository, ReactionRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IShareRepository, ShareRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IPointOfInterestRepository, PointOfInterestRepository>();
        services.AddScoped<ICheckInRepository, CheckInRepository>();
        services.AddScoped<IRouteRepository, RouteRepository>();
        services.AddScoped<INewsRepository, NewsRepository>();
        services.AddScoped<IReviewsRepository, ReviewsRepository>();
        services.AddScoped<IStoriesRepository, StoriesRepository>();
        services.AddScoped<IRatingRepository, RatingRepository>();
        services.AddScoped<IEventsRepository, EventsRepository>();
        services.AddScoped<IGroupsRepository, GroupsRepository>();
        services.AddScoped<IPostsRepository, PostsRepository>();
        services.AddScoped<IFriendsRepository, FriendsRepository>();
        services.AddScoped<IGuidesRepository, GuidesRepository>();

        // AI Repositories
        services.AddScoped<IAIModelRepository, AIModelRepository>();
        services.AddScoped<ITrainingJobRepository, TrainingJobRepository>();
        services.AddScoped<ITrainingHistoryRepository, TrainingHistoryRepository>();

        // AI Services
        services.AddScoped<IAIManagementService, AIManagementService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Consolidated User Repository (replacing multiple user-related repositories)
        services.AddScoped<IUserRepository, UserRepository>();

        // Profile Repositories
        services.AddScoped<IUserGalleryRepository, UserGalleryRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IUserBadgeRepository, UserBadgeRepository>();
        services.AddScoped<IUserAchievementRepository, UserAchievementRepository>();
        services.AddScoped<IUserActivityRepository, UserActivityRepository>();
        services.AddScoped<IUserInterestRepository, UserInterestRepository>();
        services.AddScoped<IUserFollowingRepository, UserFollowingRepository>();

        // Authentication & Authorization services
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<ITwoFactorService, TwoFactorService>();
        services.AddScoped<IOAuthService, OAuthService>();

        // Focused OAuth Services
        services.AddScoped<IGoogleOAuthService, GoogleOAuthService>();
        services.AddScoped<IFacebookOAuthService, FacebookOAuthService>();

        // Focused TwoFactor Services
        services.AddScoped<IAuthenticatorService, AuthenticatorService>();
        services.AddScoped<IRecoveryCodesService, RecoveryCodesService>();

        // Focused Authentication Services
        services.AddScoped<IRegistrationService, RegistrationService>();
        services.AddScoped<ILoginService, LoginService>();
        services.AddScoped<IPasswordResetService, PasswordResetService>();

        // Focused AI Services
        services.AddScoped<IModelManagementService, ModelManagementService>();
        services.AddScoped<ITrainingManagementService, TrainingManagementService>();
        services.AddScoped<ITrainingHistoryService, TrainingHistoryService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IGuidesNotificationService, GuidesNotificationService>();
        services.AddScoped<INewsNotificationService, NewsNotificationService>();
        services.AddScoped<ISharedSearchService, SharedSearchService>();
        services.AddScoped<IContentModerationService, ContentModerationService>();

        // Storage services
        services.AddScoped<IFileStorageService, FileStorageService>();

        // System Services
        services.AddMemoryCache();
        services.AddHttpClient();
        services.AddScoped<IMaintenanceService, MaintenanceService>();

        // Caching Services
        services.AddRedisCache(configuration);
        
        // Background Job Services
        services.AddBackgroundJobs(configuration);
        services.AddScoped<GamificationBackgroundJobService>();
        services.AddScoped<MaintenanceBackgroundJobService>();
        services.AddScoped<FeedBackgroundJobService>();
        services.AddScoped<EmailBackgroundJobService>();
        services.AddScoped<BackgroundJobSchedulerService>();
        services.AddScoped<IBackgroundJobService, HangfireBackgroundJobService>();

        // Caching Services
        services.Configure<CacheSettings>(configuration.GetSection(CacheSettings.SectionName));
        services.AddScoped<ICacheService, CacheService>();
        
        // Add distributed cache if enabled
        var cacheSettings = configuration.GetSection(CacheSettings.SectionName).Get<CacheSettings>();
        if (cacheSettings?.EnableDistributedCache == true && !string.IsNullOrEmpty(cacheSettings.RedisConnectionString))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = cacheSettings.RedisConnectionString;
            });
            services.AddScoped<IDistributedCacheService, DistributedCacheService>();
        }

        // Background Jobs Services
        services.Configure<BackgroundJobSettings>(configuration.GetSection(BackgroundJobSettings.SectionName));
        services.AddScoped<IBackgroundJobService, BackgroundJobService>();
        services.AddScoped<IJobProcessor, JobProcessor>();
        
        // Add hosted service for scheduled jobs
        var jobSettings = configuration.GetSection(BackgroundJobSettings.SectionName).Get<BackgroundJobSettings>();
        if (jobSettings?.EnableScheduledJobs == true)
        {
            services.AddHostedService<ScheduledJobsHostedService>();
        }

        // Add Authentication (Cookie only)
        services.AddAuthentication();

        // Configure cookie authentication
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Login";
            options.LogoutPath = "/Logout";
            options.AccessDeniedPath = "/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromDays(30);
            options.SlidingExpiration = true;
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
        });

        // Seeding
        services.AddScoped<CommunityCar.Infrastructure.Persistence.Seeding.DataSeeder>(provider =>
            new CommunityCar.Infrastructure.Persistence.Seeding.DataSeeder(
                provider.GetRequiredService<ApplicationDbContext>(),
                provider.GetRequiredService<UserManager<User>>(),
                provider.GetRequiredService<ILogger<CommunityCar.Infrastructure.Persistence.Seeding.DataSeeder>>(),
                provider));

        return services;
    }
}

