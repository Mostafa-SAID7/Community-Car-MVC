using CommunityCar.Infrastructure.Configuration;
using CommunityCar.Infrastructure.Configuration.Account;
using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Common.Interfaces.Services.Account.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Communication;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Shared;
using CommunityCar.Application.Common.Interfaces.Data;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Application.Common.Interfaces.Repositories.Account;
using CommunityCar.Application.Common.Interfaces.Repositories.AI;
using CommunityCar.Application.Common.Interfaces.Repositories.Chat;
using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Application.Common.Interfaces.Services.Storage;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Caching;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.BackgroundJobs;
using CommunityCar.Application.Common.Interfaces.Services.Account.Authentication.OAuth;
using CommunityCar.Application.Common.Interfaces.Services.Authentication;
using CommunityCar.Application.Common.Interfaces.Repositories.Authorization;
using CommunityCar.Application.Services.Account.Authorization;
using CommunityCar.Application.Services.Shared;
using CommunityCar.Application.Services.AI;
using CommunityCar.Application.Services.AI.ModelManagement;
using CommunityCar.Application.Services.AI.Training;
using CommunityCar.Application.Services.AI.History;
using CommunityCar.Application.Services.Account;
using CommunityCar.Application.Services.Account.Authentication;
using CommunityCar.Application.Services.Account.Authentication.OAuth;
using CommunityCar.Application.Services.Communication;
using CommunityCar.Application.Services.Community;
using CommunityCar.Application.Services.Storage;
using CommunityCar.Application.Services.Dashboard;
using CommunityCar.Application.Services.Dashboard.Caching;
using CommunityCar.Application.Common.Interfaces.Hubs;
using CommunityCar.Infrastructure.Hubs;
using CommunityCar.Domain.Entities.Account.Core;
using CommunityCar.Domain.Entities.Account.Authorization;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using CommunityCar.Infrastructure.Persistence.Repositories.Community;
using CommunityCar.Infrastructure.Persistence.Repositories.Account.Core;
using CommunityCar.Infrastructure.Persistence.Repositories.Account.Authentication;
using CommunityCar.Infrastructure.Persistence.Repositories.Account.Activity;
using CommunityCar.Infrastructure.Persistence.Repositories.Account.Gamification;
using CommunityCar.Infrastructure.Persistence.Repositories.Account.Social;
using CommunityCar.Infrastructure.Persistence.Repositories.Account.Media;
using CommunityCar.Infrastructure.Persistence.Repositories.Account.Management;
using CommunityCar.Infrastructure.Persistence.Repositories.Account.Authorization;
using CommunityCar.Infrastructure.Persistence.Repositories.Shared;
using CommunityCar.Infrastructure.Persistence.Repositories.AI;
using CommunityCar.Infrastructure.Persistence.Repositories.Chat;
using CommunityCar.Infrastructure.Persistence.UnitOfWork;

using CommunityCar.Infrastructure.BackgroundJobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace CommunityCar.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure Account settings
        services.AddAccountConfiguration(configuration);
        AccountConfigurationExtensions.ValidateAccountConfiguration(configuration);

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString,
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
                    .EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null)));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddIdentity<User, Role>(options => {
            // Password policy
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequiredUniqueChars = 4;

            // Lockout settings - Relaxed for testing
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
            options.Lockout.MaxFailedAccessAttempts = 999;
            options.Lockout.AllowedForNewUsers = false;

            // User settings
            options.User.RequireUniqueEmail = true;
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

            // Email confirmation
            options.SignIn.RequireConfirmedEmail = false; // Allow external logins without email confirmation
            options.SignIn.RequireConfirmedAccount = false;

            // Token settings
            options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
            options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/login");
            options.LogoutPath = new Microsoft.AspNetCore.Http.PathString("/logout");
            options.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/access-denied");
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromDays(30);
            options.SlidingExpiration = true;
        });

        // Configure external authentication providers
        services.AddAuthentication()
            .AddGoogle(options =>
            {
                var googleSettings = configuration.GetSection("SocialAuth:Google");
                options.ClientId = googleSettings["ClientId"] ?? "";
                options.ClientSecret = googleSettings["ClientSecret"] ?? "";
                options.SaveTokens = true;
                options.Scope.Add("profile");
                options.Scope.Add("email");
            })
            .AddFacebook(options =>
            {
                var facebookSettings = configuration.GetSection("SocialAuth:Facebook");
                options.AppId = facebookSettings["AppId"] ?? "";
                options.AppSecret = facebookSettings["AppSecret"] ?? "";
                options.SaveTokens = true;
                options.Scope.Add("email");
                options.Scope.Add("public_profile");
            });

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
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();

        // AI Repositories
        services.AddScoped<IAIModelRepository, AIModelRepository>();
        services.AddScoped<ITrainingJobRepository, TrainingJobRepository>();
        services.AddScoped<ITrainingHistoryRepository, TrainingHistoryRepository>();

        // AI Services
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Consolidated User Repository (replacing multiple user-related repositories)
        // Consolidated User Repository (replacing multiple user-related repositories)
        services.AddScoped<IUserRepository, UserRepository>();


        // Profile Repositories
        services.AddScoped<IUserGalleryRepository, UserGalleryRepository>();
        services.AddScoped<IUserBadgeRepository, UserBadgeRepository>();
        services.AddScoped<IUserAchievementRepository, UserAchievementRepository>();
        services.AddScoped<IUserActivityRepository, UserActivityRepository>();
        services.AddScoped<IUserInterestRepository, UserInterestRepository>();
        services.AddScoped<IUserFollowingRepository, UserFollowingRepository>();
        services.AddScoped<IUserProfileViewRepository, UserProfileViewRepository>();

        // Authorization Repositories
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();


        // SignalR Hub Context Wrapper
        services.AddScoped<INotificationHubContext, NotificationHubContextWrapper>();
        
        // Broadcast Service
        services.AddScoped<IBroadcastService, Services.BroadcastService>();

        // System Services
        services.AddMemoryCache();
        services.AddHttpClient();

        // Caching Services - Configure Redis if enabled
        services.AddRedisCache(configuration);
        services.AddScoped<CacheWarmupService>();
        
        // Background Job Services - only add if enabled
        var backgroundJobSettings = configuration.GetSection("BackgroundJobs").Get<BackgroundJobSettings>();
        if (backgroundJobSettings?.EnableScheduledJobs == true)
        {
            services.AddBackgroundJobs(configuration);
        }
        
        services.AddScoped<IBackgroundJobService, BackgroundJobs.BackgroundJobService>();

        // Caching Configuration
        services.Configure<CacheSettings>(configuration.GetSection(CacheSettings.SectionName));
        
        // Add distributed cache if enabled
        var cacheSettings = configuration.GetSection(CacheSettings.SectionName).Get<CacheSettings>();
        if (cacheSettings?.EnableDistributedCache == true && !string.IsNullOrEmpty(cacheSettings.RedisConnectionString))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = cacheSettings.RedisConnectionString;
            });
        }

        // Background Jobs Services
        services.Configure<BackgroundJobSettings>(configuration.GetSection(BackgroundJobSettings.SectionName));
        services.AddScoped<IJobProcessor, JobProcessor>();
        
        // Add hosted service for scheduled jobs
        var jobSettings = configuration.GetSection(BackgroundJobSettings.SectionName).Get<BackgroundJobSettings>();
        if (jobSettings?.EnableScheduledJobs == true)
        {
            services.AddHostedService<ScheduledJobsHostedService>();
        }


        // Seeding
        services.AddScoped<CommunityCar.Infrastructure.Persistence.Seeding.DataSeeder>(provider =>
            new CommunityCar.Infrastructure.Persistence.Seeding.DataSeeder(
                provider.GetRequiredService<ApplicationDbContext>(),
                provider.GetRequiredService<UserManager<User>>(),
                provider.GetRequiredService<RoleManager<Role>>(),
                provider.GetRequiredService<ILogger<CommunityCar.Infrastructure.Persistence.Seeding.DataSeeder>>(),
                provider));

        return services;
    }
}

